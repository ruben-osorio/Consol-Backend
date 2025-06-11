using Consolidado.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consolidado.Services
{
    public interface IUnidadOrganizacionalService
    {
        Task<IEnumerable<UnidadOrganizacional>> GetAllAsync();
        Task<UnidadOrganizacional?> GetByIdAsync(int id);
    }
}