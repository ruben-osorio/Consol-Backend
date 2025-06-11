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
    /// <summary>
    /// Servicio para gestionar las asistencias del personal
    /// </summary>
    public interface IAsistenciaService
    {
        /// <summary>
        /// Obtiene el registro de asistencias de un empleado por su CI
        /// </summary>
        /// <param name="ci">Código de identificación del empleado</param>
        /// <param name="startDate">Fecha de inicio del período de búsqueda</param>
        /// <param name="endDate">Fecha de fin del período de búsqueda</param>
        /// <returns>Lista de registros de asistencia del empleado</returns>
        Task<IEnumerable<AsistenciaModel>> GetAsistencias(string ci, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene un resumen pivotado de las asistencias por departamento
        /// </summary>
        /// <param name="departmentName">Nombre del departamento</param>
        /// <param name="startDate">Fecha de inicio del período de búsqueda</param>
        /// <param name="endDate">Fecha de fin del período de búsqueda</param>
        /// <returns>Datos pivotados de asistencia del departamento</returns>
        Task<AsistenciaPivotData> GetAsistenciasPivot(string departmentName, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene la planilla de asistencia por departamento
        /// </summary>
        /// <param name="departmentName">Nombre del departamento</param>
        /// <param name="startDate">Fecha de inicio del período de búsqueda</param>
        /// <param name="endDate">Fecha de fin del período de búsqueda</param>
        /// <returns>Planilla de asistencia del departamento</returns>
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

        public async Task<IEnumerable<AsistenciaModel>> GetAsistencias(string ci, DateTime startDate, DateTime endDate)
        {
            var asistencias = new List<AsistenciaModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_get_asistencias_ci", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        // Agregar parámetros con tipos específicos
                        command.Parameters.Add("@ci", SqlDbType.VarChar, 20).Value = ci;
                        command.Parameters.Add("@start_date", SqlDbType.Date).Value = startDate.Date;
                        command.Parameters.Add("@end_date", SqlDbType.Date).Value = endDate.Date;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    var asistencia = new AsistenciaModel
                                    {
                                        CodigoDepartamento = reader["CodigoDepartamento"]?.ToString(),
                                        NombreDepartamento = reader["NombreDepartamento"]?.ToString(),
                                        IdCarnet = reader["idcarnet"]?.ToString(),
                                        Nombre = reader["nombre"]?.ToString(),
                                        ApellidoPaterno = reader["apellido_paterno"]?.ToString(),
                                        ApellidoMaterno = reader["apellido_materno"]?.ToString(),
                                        Fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue,
                                        Dia = reader["dia"]?.ToString(),
                                        IdHorario = reader["id_horario"] != DBNull.Value ? Convert.ToInt32(reader["id_horario"]) : 0,
                                        Horario = reader["horario"]?.ToString(),
                                        DiaTrabajo = reader["diatrabajo"] != DBNull.Value ? Convert.ToDecimal(reader["diatrabajo"]) : 0,
                                        MEntrada = reader["mentrada"]?.ToString(),
                                        MSalida = reader["msalida"]?.ToString(),
                                        Atraso = reader["atraso"] != DBNull.Value ? Convert.ToInt32(reader["atraso"]) : 0,
                                        SumatoriaTotalAtrasos = reader["Sumatoria Total Atrasos"] != DBNull.Value ? Convert.ToInt32(reader["Sumatoria Total Atrasos"]) : 0,
                                        Temprano = reader["temprano"] != DBNull.Value ? Convert.ToInt32(reader["temprano"]) : 0,
                                        Falta = reader["falta"] != DBNull.Value ? Convert.ToInt32(reader["falta"]) : 0,
                                        Feriado = reader["feriado"]?.ToString(),
                                        Bingreso = reader["bingreso"]?.ToString(),
                                        Bsalida = reader["bsalida"]?.ToString()
                                    };
                                    asistencias.Add(asistencia);
                                }
                                catch (Exception ex)
                                {
                                    // Log del error específico al leer una fila
                                    throw new Exception($"Error al procesar fila: {ex.Message}. CI: {ci}, Fecha: {startDate:yyyy-MM-dd} a {endDate:yyyy-MM-dd}");
                                }
                            }
                        }
                    }
                }

                return asistencias.ToList();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error de SQL: {ex.Message}. Número: {ex.Number}, Estado: {ex.State}, Procedimiento: {ex.Procedure}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener asistencias: {ex.Message}");
            }
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
               
                    };
                }

                var fechaKey = asistencia.Fecha.ToString("dd/MM/yyyy");
                
                // Verificar si ya existe un marcado para esta fecha
                if (!planillas[asistencia.IdCarnet].Marcados.ContainsKey(fechaKey))
                {
                    planillas[asistencia.IdCarnet].Marcados[fechaKey] = new MarcadoDiario
                    {
                        Fecha = fechaKey,
                        MinutosAtraso = 0 // Se calculará la suma total después
                    };
                }
                
                var marcado = planillas[asistencia.IdCarnet].Marcados[fechaKey];
                
                // Agregar información del turno actual con sus respectivas entradas y salidas
                if (!string.IsNullOrEmpty(asistencia.Horario))
                {
                    var turno = new MarcadoTurno
                    {
                        id_turno = asistencia.IdHorario,
                        NombreTurno = asistencia.Horario,
                        HoraEntrada = asistencia.MEntrada,
                        HoraSalida = asistencia.MSalida,
                        MinutosAtraso = asistencia.Atraso
                    };
                    
                    marcado.Turnos.Add(turno);
                }
                
                // Actualizar el atraso total para esta fecha
                marcado.MinutosAtraso += asistencia.Atraso;
                
                // Determinar el estado del marcado
                marcado.Estado = DeterminarEstado(asistencia);
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