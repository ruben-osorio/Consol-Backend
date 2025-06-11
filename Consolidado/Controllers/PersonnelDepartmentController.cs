using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Consolidado.Models;
using Consolidado.Services;

namespace Consolidado.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelDepartmentController : ControllerBase
    {
        private readonly IPersonnelDepartmentService _departmentService;

        public PersonnelDepartmentController(IPersonnelDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonnelDepartment>>> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonnelDepartment>> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        [HttpGet("code/{deptCode}")]
        public async Task<ActionResult<PersonnelDepartment>> GetDepartmentByCode(string deptCode)
        {
            var department = await _departmentService.GetDepartmentByCodeAsync(deptCode);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }
    }
} 