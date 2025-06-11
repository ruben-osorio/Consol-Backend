using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Consolidado.Models;
using Consolidado.Data;

namespace Consolidado.Services
{
    public class PersonnelDepartmentService : IPersonnelDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public PersonnelDepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonnelDepartment>> GetAllDepartmentsAsync()
        {
            return await _context.PersonnelDepartments!
                .OrderBy(d => d.DeptName)
                .ToListAsync();
        }

        public async Task<PersonnelDepartment?> GetDepartmentByIdAsync(int id)
        {
            return await _context.PersonnelDepartments!
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<PersonnelDepartment?> GetDepartmentByCodeAsync(string deptCode)
        {
            return await _context.PersonnelDepartments!
                .FirstOrDefaultAsync(d => d.DeptCode == deptCode);
        }
    }
} 