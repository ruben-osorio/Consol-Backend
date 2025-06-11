// Controllers/UnidadesOrganizacionalesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Consolidado.Services;
using Consolidado.Models;

namespace Consolidado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadesOrganizacionalesController : ControllerBase
    {
        private readonly IUnidadOrganizacionalService _service;

        public UnidadesOrganizacionalesController(IUnidadOrganizacionalService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todas las unidades organizacionales
        /// </summary>
        /// <returns>Lista de unidades organizacionales</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnidadOrganizacional>>> Get()
        {
            var unidades = await _service.GetAllAsync();
            return Ok(unidades);
        }

        /// <summary>
        /// Obtiene una unidad organizacional por su ID
        /// </summary>
        /// <param name="id">ID de la unidad organizacional</param>
        /// <returns>Unidad organizacional encontrada</returns>
        /// <response code="200">Si la unidad organizacional fue encontrada</response>
        /// <response code="404">Si no se encuentra la unidad organizacional</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UnidadOrganizacional>> GetById(int id)
        {
            var unidad = await _service.GetByIdAsync(id);
            
            if (unidad == null)
            {
                return NotFound();
            }
            
            return Ok(unidad);
        }
    }
}
