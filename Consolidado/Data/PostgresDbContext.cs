using Microsoft.EntityFrameworkCore;
using Consolidado.Models;

namespace Consolidado.Data
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options) { }

        public DbSet<UnidadOrganizacional> UnidadesOrganizacionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<UnidadOrganizacional>()
                .ToTable("plantillauo")
                .HasKey(u => u.Id);
        }
    }
}
