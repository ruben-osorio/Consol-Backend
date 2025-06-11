using Microsoft.EntityFrameworkCore;
using Consolidado.Models;

namespace Consolidado.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PersonnelDepartment> PersonnelDepartments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí puedes agregar configuraciones adicionales para tus entidades
            modelBuilder.Entity<PersonnelDepartment>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<PersonnelDepartment>()
                .Property(d => d.DeptCode)
                .IsRequired()
                .HasColumnName("dept_code");

            modelBuilder.Entity<PersonnelDepartment>()
                .Property(d => d.DeptName)
                .IsRequired()
                .HasColumnName("dept_name");

            modelBuilder.Entity<PersonnelDepartment>()
                .Property(d => d.IsDefault)
                .HasColumnName("is_default");

            modelBuilder.Entity<PersonnelDepartment>()
                .Property(d => d.CompanyId)
                .HasColumnName("company_id");

            modelBuilder.Entity<PersonnelDepartment>()
                .Property(d => d.ParentDeptId)
                .HasColumnName("parent_dept_id");

            modelBuilder.Entity<PersonnelDepartment>()
                .ToTable("personnel_department");
        }
    }
} 