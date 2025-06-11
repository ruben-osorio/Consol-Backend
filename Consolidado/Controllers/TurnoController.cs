using System;
using System.Threading.Tasks;
using Consolidado.Models;
using Consolidado.Services;
using Microsoft.AspNetCore.Mvc;

namespace Consolidado.Controllers
{
    /// <summary>
    /// Controlador para la gestión de turnos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoService _turnoService;

        public TurnoController(ITurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        /// <summary>
        /// Obtiene la lista de todos los turnos
        /// </summary>
        /// <returns>Lista de turnos</returns>
        /// <response code="200">Retorna la lista de turnos</response>
        /// <response code="400">Si ocurre un error al procesar la solicitud</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TurnoModel>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetTurnos()
        {
            try
            {
                var result = await _turnoService.GetTurnos();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un turno específico por su ID
        /// </summary>
        /// <param name="id">ID del turno</param>
        /// <returns>Información del turno</returns>
        /// <response code="200">Retorna el turno solicitado</response>
        /// <response code="404">Si no se encuentra el turno</response>
        /// <response code="400">Si ocurre un error al procesar la solicitud</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TurnoModel), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetTurnoById(int id)
        {
            try
            {
                var result = await _turnoService.GetTurnoById(id);
                if (result == null)
                {
                    return NotFound(new { message = "No se encontró el turno especificado" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sincroniza los turnos con el sistema externo
        /// </summary>
        /// <param name="request">Solicitud de sincronización</param>
        /// <returns>Resultado de la sincronización</returns>
        /// <response code="200">Si la sincronización fue exitosa</response>
        /// <response code="400">Si la solicitud es inválida o ocurre un error</response>
        [HttpPost("sincronizar")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> SincronizarTurnos([FromBody] TurnoRequest request)
        {
            try
            {
                if (!request.SincronizarTurnos)
                {
                    return BadRequest(new { message = "La solicitud no indica sincronización de turnos" });
                }

                var result = await _turnoService.SincronizarTurnos();
                if (result)
                {
                    return Ok(new { message = "Turnos sincronizados correctamente" });
                }
                else
                {
                    return BadRequest(new { message = "Error al sincronizar turnos" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la definición de un turno específico
        /// </summary>
        /// <param name="idTurno">ID del turno a actualizar</param>
        /// <param name="request">Nueva definición del turno</param>
        /// <returns>Resultado de la actualización</returns>
        /// <response code="200">Si la actualización fue exitosa</response>
        /// <response code="404">Si no se encuentra el turno</response>
        /// <response code="400">Si la solicitud es inválida o ocurre un error</response>
        [HttpPut("{idTurno}/definicion")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ActualizarDefinicionTurno(int idTurno, [FromBody] ActualizarDefinicionTurnoRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Definicion))
                {
                    return BadRequest(new { message = "La definición no puede estar vacía" });
                }

                var result = await _turnoService.ActualizarDefinicionTurno(idTurno, request.Definicion);
                if (result)
                {
                    return Ok(new { message = "Definición actualizada correctamente" });
                }
                else
                {
                    return NotFound(new { message = "No se encontró el turno especificado" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 