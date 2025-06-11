using System;

namespace Consolidado.Models
{
    /// <summary>
    /// Modelo base para las solicitudes de asistencia
    /// </summary>
    public class AsistenciaRequestBase
    {
        /// <summary>
        /// Fecha de inicio para el rango de búsqueda. Si no se especifica, se usa la fecha de hace 30 días.
        /// </summary>
        /// <example>2024-01-01</example>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Fecha de fin para el rango de búsqueda. Si no se especifica, se usa la fecha actual.
        /// </summary>
        /// <example>2024-01-31</example>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Modelo para solicitudes de asistencia por CI
    /// </summary>
    public class AsistenciaRequest : AsistenciaRequestBase
    {
        /// <summary>
        /// Código de identificación del empleado (CI)
        /// </summary>
        /// <example>4821575</example>
        public string? CI { get; set; }
    }

    /// <summary>
    /// Modelo para solicitudes de asistencia por departamento
    /// </summary>
    public class AsistenciaDepartamentoRequest : AsistenciaRequestBase
    {
        /// <summary>
        /// Nombre del departamento
        /// </summary>
        /// <example>Recursos Humanos</example>
        public string? NombreDepartamento { get; set; }
    }
}
