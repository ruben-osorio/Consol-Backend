using System;
using System.Collections.Generic;

namespace Consolidado.Models
{
    public class AsistenciaModel
    {
        public string? CodigoDepartamento { get; set; }
        public string? NombreDepartamento { get; set; }
        public string? IdCarnet { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public DateTime Fecha { get; set; }
        public string? Dia { get; set; }
        public string? Horario { get; set; }
        public decimal DiaTrabajo { get; set; }
        public string? MEntrada { get; set; }
        public string? MSalida { get; set; }
        public int Atraso { get; set; }
        public int SumatoriaTotalAtrasos { get; set; }
    }

    public class AsistenciaRequest
    {
        public string? NombreDepartamento { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
        public string? Turno { get; set; }
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
        public string? NombreTurno { get; set; }
        public string? HoraEntrada { get; set; }
        public string? HoraSalida { get; set; }
        public int MinutosAtraso { get; set; }
    }
} 