using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using Consolidado.Models;
using Microsoft.Extensions.Configuration;
using NReco.PivotData;

namespace Consolidado.Services
{
    public interface IAsistenciaService
    {
        Task<IEnumerable<AsistenciaModel>> GetAsistencias(string departmentName, DateTime startDate, DateTime endDate);
        Task<AsistenciaPivotData> GetAsistenciasPivot(string departmentName, DateTime startDate, DateTime endDate);
        Task<IEnumerable<PlanillaAsistenciaModel>> GetPlanillaAsistencia(string departmentName, DateTime startDate, DateTime endDate);
    }

    public class AsistenciaService : IAsistenciaService
    {
        private readonly string? _connectionString;

        public AsistenciaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "La cadena de conexión no puede ser nula");
        }

        public async Task<IEnumerable<AsistenciaModel>> GetAsistencias(string NombreDepartamento, DateTime startDate, DateTime endDate)
        {
            var asistencias = new List<AsistenciaModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_get_asistencias", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@dept_name", NombreDepartamento);
                    command.Parameters.AddWithValue("@start_date", startDate);
                    command.Parameters.AddWithValue("@end_date", endDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            asistencias.Add(new AsistenciaModel
                            {
                                CodigoDepartamento = reader["CodigoDepartamento"].ToString(),
                                NombreDepartamento = reader["NombreDepartamento"].ToString(),
                                IdCarnet = reader["idcarnet"].ToString(),
                                Nombre = reader["nombre"]?.ToString(),
                                ApellidoPaterno = reader["apellido_paterno"]?.ToString(),
                                ApellidoMaterno = reader["apellido_materno"]?.ToString(),
                                Fecha = Convert.ToDateTime(reader["fecha"]),
                                Dia = reader["dia"].ToString(),
                                Horario = reader["horario"].ToString(),
                                DiaTrabajo = Convert.ToDecimal(reader["diatrabajo"]),
                                MEntrada = reader["mentrada"]?.ToString(),
                                MSalida = reader["msalida"]?.ToString(),
                                Atraso = Convert.ToInt32(reader["atraso"]),
                                SumatoriaTotalAtrasos = Convert.ToInt32(reader["Sumatoria Total Atrasos"])
                            });
                        }
                    }
                }
            }

            return asistencias.Where(a => !(string.IsNullOrEmpty(a.MEntrada) && string.IsNullOrEmpty(a.MSalida))).ToList();
        }

        public async Task<AsistenciaPivotData> GetAsistenciasPivot(string departmentName, DateTime startDate, DateTime endDate)
        {
            var asistencias = await GetAsistencias(departmentName, startDate, endDate);
            
            var pivotData = new PivotData(
                new[] { "IdCarnet", "Fecha" },
                new CompositeAggregatorFactory(
                    new CountAggregatorFactory(),
                    new SumAggregatorFactory("Atraso")
                )
            );

            foreach (var asistencia in asistencias)
            {
                if (asistencia.IdCarnet != null)
                {
                    pivotData.ProcessData(new[] { new Dictionary<string, object>
                    {
                        { "IdCarnet", asistencia.IdCarnet },
                        { "Fecha", asistencia.Fecha.ToString("yyyy-MM-dd") },
                        { "Atraso", asistencia.Atraso }
                    }});
                }
            }

            var dimensions = new[] { "IdCarnet", "Fecha" };
            var values = new List<object>();
            var headers = new List<string>();

            foreach (var entry in pivotData)
            {
                var key = entry.Key;
                var value = entry.Value;
                values.Add(new
                {
                    IdCarnet = key[0],
                    Fecha = key[1],
                    Count = value.AsComposite().Aggregators[0].Value,
                    SumAtraso = value.AsComposite().Aggregators[1].Value
                });
            }

            return new AsistenciaPivotData
            {
                Dimensions = dimensions,
                Values = values.ToArray(),
                Headers = headers.ToArray()
            };
        }

        public async Task<IEnumerable<PlanillaAsistenciaModel>> GetPlanillaAsistencia(string departmentName, DateTime startDate, DateTime endDate)
        {
            var asistencias = await GetAsistencias(departmentName, startDate, endDate);
            var planillas = new Dictionary<string, PlanillaAsistenciaModel>();

            foreach (var asistencia in asistencias)
            {
                if (string.IsNullOrEmpty(asistencia.IdCarnet))
                    continue;

                if (string.IsNullOrEmpty(asistencia.MEntrada) && string.IsNullOrEmpty(asistencia.MSalida))
                    continue;

                if (!planillas.ContainsKey(asistencia.IdCarnet))
                {
                    if (string.IsNullOrEmpty(asistencia.ApellidoPaterno))
                    {
                        asistencia.ApellidoPaterno = "SIN APELLIDO";
                    }
                    if (string.IsNullOrEmpty(asistencia.Nombre))
                    {
                        asistencia.Nombre = "SIN NOMBRE";
                    }
                    if (string.IsNullOrEmpty(asistencia.IdCarnet))
                    {
                        asistencia.IdCarnet = "SIN CI";
                    }

                    planillas[asistencia.IdCarnet] = new PlanillaAsistenciaModel
                    {
                        PrimerApellido = asistencia.ApellidoPaterno,
                        SegundoApellido = asistencia.ApellidoMaterno,
                        Nombres = asistencia.Nombre,
                        CI = asistencia.IdCarnet,
                        EXP = "", // Este campo deberá ser agregado a la base de datos si es necesario
                        Turno = asistencia.Horario
                    };
                }

                var fechaKey = asistencia.Fecha.ToString("dd/MM/yyyy");
                
                // Verificar si ya existe un marcado para esta fecha
                if (!planillas[asistencia.IdCarnet].Marcados.ContainsKey(fechaKey))
                {
                    planillas[asistencia.IdCarnet].Marcados[fechaKey] = new MarcadoDiario
                    {
                        Fecha = fechaKey,
                        MinutosAtraso = 0
                    };
                }
                
                var marcado = planillas[asistencia.IdCarnet].Marcados[fechaKey];
                
                // Crear un nuevo turno con sus marcaciones
                var turno = new MarcadoTurno
                {
                    NombreTurno = asistencia.Horario,
                    HoraEntrada = asistencia.MEntrada,
                    HoraSalida = asistencia.MSalida,
                    MinutosAtraso = asistencia.Atraso
                };

                // Agregar el turno a la lista
                marcado.Turnos.Add(turno);
                
                // Actualizar el atraso total para esta fecha
                marcado.MinutosAtraso += asistencia.Atraso;
                
                // Determinar el estado del marcado
                marcado.Estado = DeterminarEstado(asistencia);
            }

            // Ordenar cronológicamente los turnos para cada marcado diario
            foreach (var planilla in planillas.Values)
            {
                foreach (var marcadoPair in planilla.Marcados)
                {
                    var marcado = marcadoPair.Value;
                    // Ordenar los turnos por hora de entrada (de temprano a tarde)
                    marcado.Turnos = marcado.Turnos
                        .OrderBy(t => string.IsNullOrEmpty(t.HoraEntrada) ? "23:59" : t.HoraEntrada)
                        .ToList();
                }
            }

            return planillas.Values;
        }

        private string DeterminarEstado(AsistenciaModel asistencia)
        {
            if (string.IsNullOrEmpty(asistencia.MEntrada) && string.IsNullOrEmpty(asistencia.MSalida))
                return "F"; // Falta
            if (asistencia.Atraso > 0)
                return "A"; // Atraso
            if (!string.IsNullOrEmpty(asistencia.MEntrada) && !string.IsNullOrEmpty(asistencia.MSalida))
                return "P"; // Presente
            return "I"; // Incompleto (solo entrada o solo salida)
        }
    }
} 