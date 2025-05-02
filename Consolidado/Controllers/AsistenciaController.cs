using System;
using System.Threading.Tasks;
using Consolidado.Models;
using Consolidado.Services;
using Microsoft.AspNetCore.Mvc;

namespace Consolidado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciaController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsistencias([FromQuery] AsistenciaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NombreDepartamento))
                {
                    return BadRequest(new { message = "El nombre del departamento es requerido" });
                }

                var result = await _asistenciaService.GetAsistencias(
                    request.NombreDepartamento,
                    request.StartDate,
                    request.EndDate
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pivot")]
        public async Task<IActionResult> GetAsistenciasPivot([FromQuery] AsistenciaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NombreDepartamento))
                {
                    return BadRequest(new { message = "El nombre del departamento es requerido" });
                }

                var result = await _asistenciaService.GetAsistenciasPivot(
                    request.NombreDepartamento,
                    request.StartDate,
                    request.EndDate
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("planilla")]
        public async Task<IActionResult> GetPlanillaAsistencia([FromQuery] AsistenciaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NombreDepartamento))
                {
                    return BadRequest(new { message = "El nombre del departamento es requerido" });
                }

                var result = await _asistenciaService.GetPlanillaAsistencia(
                    request.NombreDepartamento,
                    request.StartDate,
                    request.EndDate
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