using System;

namespace Consolidado.Models
{
    /// <summary>
    /// Modelo que representa un turno
    /// </summary>
    public class TurnoModel
    {
        /// <summary>
        /// Identificador único del turno
        /// </summary>
        public int IdTurno { get; set; }

        /// <summary>
        /// Alias o nombre corto del turno
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// Definición detallada del turno
        /// </summary>
        public string? Definicion { get; set; }

        /// <summary>
        /// Margen de adelanto para la entrada (en minutos)
        /// </summary>
        public int InAheadMargin { get; set; }

        /// <summary>
        /// Margen de tolerancia para la entrada (en minutos)
        /// </summary>
        public int InAboveMargin { get; set; }

        /// <summary>
        /// Margen de adelanto para la salida (en minutos)
        /// </summary>
        public int OutAheadMargin { get; set; }

        /// <summary>
        /// Margen de tolerancia para la salida (en minutos)
        /// </summary>
        public int OutAboveMargin { get; set; }

        /// <summary>
        /// Duración del turno (en minutos)
        /// </summary>
        public int Duration { get; set; }
    }

    /// <summary>
    /// Modelo para la solicitud de sincronización de turnos
    /// </summary>
    public class TurnoRequest
    {
        /// <summary>
        /// Indica si se debe sincronizar los turnos
        /// </summary>
        public bool SincronizarTurnos { get; set; }
    }

    /// <summary>
    /// Modelo para la solicitud de actualización de definición de turno
    /// </summary>
    public class ActualizarDefinicionTurnoRequest
    {
        /// <summary>
        /// Nueva definición del turno
        /// </summary>
        public required string Definicion { get; set; }
    }
} 