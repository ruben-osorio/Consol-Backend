using System;
using System.Collections.Generic;

namespace Consolidado.Models
{
    /// <summary>
    /// Modelo que representa un registro de asistencia
    /// </summary>
    public class AsistenciaModel
    {
        /// <summary>
        /// Código del departamento
        /// </summary>
        public string? CodigoDepartamento { get; set; }

        /// <summary>
        /// Nombre del departamento
        /// </summary>
        public string? NombreDepartamento { get; set; }

        /// <summary>
        /// Código de identificación del empleado
        /// </summary>
        public string? IdCarnet { get; set; }

        /// <summary>
        /// Nombre del empleado
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Apellido paterno del empleado
        /// </summary>
        public string? ApellidoPaterno { get; set; }

        /// <summary>
        /// Apellido materno del empleado
        /// </summary>
        public string? ApellidoMaterno { get; set; }

        /// <summary>
        /// Fecha del registro de asistencia
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Día de la semana
        /// </summary>
        public string? Dia { get; set; }

        /// <summary>
        /// Identificador del horario
        /// </summary>
        public int IdHorario { get; set; }

        /// <summary>
        /// Descripción del horario
        /// </summary>
        public string? Horario { get; set; }

        /// <summary>
        /// Día de trabajo (1.0 = día completo, 0.5 = medio día)
        /// </summary>
        public decimal DiaTrabajo { get; set; }

        /// <summary>
        /// Hora de entrada (formato HH:mm)
        /// </summary>
        public string? MEntrada { get; set; }

        /// <summary>
        /// Hora de salida (formato HH:mm)
        /// </summary>
        public string? MSalida { get; set; }

        /// <summary>
        /// Minutos de atraso
        /// </summary>
        public int Atraso { get; set; }

        /// <summary>
        /// Sumatoria total de atrasos
        /// </summary>
        public int SumatoriaTotalAtrasos { get; set; }

        /// <summary>
        /// Minutos de salida temprana
        /// </summary>
        public int Temprano { get; set; }

        /// <summary>
        /// Minutos de falta
        /// </summary>
        public int Falta { get; set; }

        /// <summary>
        /// Nombre del feriado si aplica
        /// </summary>
        public string? Feriado { get; set; }

        /// <summary>
        /// Símbolo de beneficio de ingreso
        /// </summary>
        public string? Bingreso { get; set; }

        /// <summary>
        /// Símbolo de beneficio de salida
        /// </summary>
        public string? Bsalida { get; set; }
    }


    public class AsistenciaPivotData
    {
        public string[]? Dimensions { get; set; }
        public object[]? Values { get; set; }
        public string[]? Headers { get; set; }
    }

    public class PlanillaAsistenciaModel
    {
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public string? Nombres { get; set; }
        public string? CI { get; set; }
        public string? EXP { get; set; }
    
        public Dictionary<string, MarcadoDiario> Marcados { get; set; } = new();
    }

    public class MarcadoDiario
    {
        public string? Fecha { get; set; }
        public List<MarcadoTurno> Turnos { get; set; } = new();
        public string? Estado { get; set; }
        public int MinutosAtraso { get; set; }
    }

    public class MarcadoTurno
    {
        public int id_turno { get; set; }
        public string? NombreTurno { get; set; }
        public string? HoraEntrada { get; set; }
        public string? HoraSalida { get; set; }
        public int MinutosAtraso { get; set; }
    }
} 