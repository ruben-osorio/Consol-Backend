using System.Collections.Generic;
using System.Threading.Tasks;
using Consolidado.Models;

namespace Consolidado.Services
{
    public interface IPersonnelDepartmentService
    {
        Task<IEnumerable<PersonnelDepartment>> GetAllDepartmentsAsync();
        Task<PersonnelDepartment?> GetDepartmentByIdAsync(int id);
        Task<PersonnelDepartment?> GetDepartmentByCodeAsync(string deptCode);
    }
} 