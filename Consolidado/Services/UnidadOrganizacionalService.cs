using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Consolidado.Data;
using Consolidado.Models;


namespace Consolidado.Services
{
    public class UnidadOrganizacionalService : IUnidadOrganizacionalService
    {
        private readonly PostgresDbContext _context;

        public UnidadOrganizacionalService(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnidadOrganizacional>> GetAllAsync()
        {
            return await _context.UnidadesOrganizacionales.ToListAsync();
        }

        public async Task<UnidadOrganizacional?> GetByIdAsync(int id)
        {
            return await _context.UnidadesOrganizacionales.FindAsync(id);
        }
    }
}