using System;
using System.Threading.Tasks;
using Consolidado.Models;
using Consolidado.Services;
using Microsoft.AspNetCore.Mvc;

namespace Consolidado.Controllers
{
    /// <summary>
    /// Controlador para gestionar las asistencias del personal
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciaController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        /// <summary>
        /// Obtiene el registro de asistencias de un empleado por su CI
        /// </summary>
        /// <param name="request">Parámetros de búsqueda que incluyen CI y rango de fechas</param>
        /// <returns>Lista de registros de asistencia del empleado</returns>
        /// <response code="200">Retorna la lista de asistencias</response>
        /// <response code="400">Si el CI es nulo o vacío</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAsistencias([FromQuery] AsistenciaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CI))
                {
                    return BadRequest(new { message = "El código de identificación (CI) es requerido" });
                }

                // Validar formato del CI
                if (!int.TryParse(request.CI, out _))
                {
                    return BadRequest(new { message = "El código de identificación (CI) debe ser numérico" });
                }

                var startDate = request.StartDate ?? DateTime.Today.AddDays(-30);
                var endDate = request.EndDate ?? DateTime.Today;

                // Validar rango de fechas
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser mayor que la fecha de fin" });
                }

                // Validar que el rango no sea mayor a 1 año
                if ((endDate - startDate).TotalDays > 730)
                {
                    return BadRequest(new { message = "El rango de fechas no puede ser mayor a 2 años" });
                }

                var result = await _asistenciaService.GetAsistencias(
                    request.CI,
                    startDate,
                    endDate
                );

                if (!result.Any())
                {
                    return NotFound(new { message = $"No se encontraron registros de asistencia para el CI {request.CI} en el período especificado" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un resumen pivotado de las asistencias por departamento
        /// </summary>
        /// <param name="request">Parámetros de búsqueda que incluyen nombre del departamento y rango de fechas</param>
        /// <returns>Resumen pivotado de asistencias por departamento</returns>
        /// <response code="200">Retorna el resumen pivotado de asistencias</response>
        /// <response code="400">Si el nombre del departamento es nulo o vacío</response>
        [HttpGet("pivot")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAsistenciasPivot([FromQuery] AsistenciaDepartamentoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NombreDepartamento))
                {
                    return BadRequest(new { message = "El nombre del departamento es requerido" });
                }

                var startDate = request.StartDate ?? DateTime.Today.AddDays(-30);
                var endDate = request.EndDate ?? DateTime.Today;

                var result = await _asistenciaService.GetAsistenciasPivot(
                    request.NombreDepartamento,
                    startDate,
                    endDate
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la planilla de asistencia por departamento
        /// </summary>
        /// <param name="request">Parámetros de búsqueda que incluyen nombre del departamento y rango de fechas</param>
        /// <returns>Planilla de asistencia del departamento</returns>
        /// <response code="200">Retorna la planilla de asistencia</response>
        /// <response code="400">Si el nombre del departamento es nulo o vacío</response>
        [HttpGet("planilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPlanillaAsistencia([FromQuery] AsistenciaDepartamentoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NombreDepartamento))
                {
                    return BadRequest(new { message = "El nombre del departamento es requerido" });
                }

                var startDate = request.StartDate ?? DateTime.Today.AddDays(-30);
                var endDate = request.EndDate ?? DateTime.Today;

                var result = await _asistenciaService.GetPlanillaAsistencia(
                    request.NombreDepartamento,
                    startDate,
                    endDate
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 