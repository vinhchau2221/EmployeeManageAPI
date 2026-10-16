using Microsoft.EntityFrameworkCore;

namespace Employee.api.Models
{
    public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
    {
        public DbSet<EmployeeModel> Employees => Set<EmployeeModel>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Designation> Designations => Set<Designation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(e =>
            {
                e.Property(x => x.departmentName).IsRequired().HasMaxLength(200);
                // Unique constraint enforced at the database level
                e.HasIndex(x => x.departmentName).IsUnique();
            });

            modelBuilder.Entity<Designation>(e =>
            {
                e.Property(x => x.designationName).IsRequired().HasMaxLength(200);
                // Unique constraint enforced at the database level
                e.HasIndex(x => x.designationName).IsUnique();

                // Designation -> Department foreign key (cannot delete a department that is in use)
                e.HasOne<Department>()
                 .WithMany()
                 .HasForeignKey(x => x.departmentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeModel>(e =>
            {
                // Unique email enforced at the database level
                e.HasIndex(x => x.email).IsUnique();

                // Unique contactNo enforced at the database level
                e.HasIndex(x => x.contactNo).IsUnique();

                // Employee -> Designation foreign key (cannot delete a designation that is in use)
                e.HasOne<Designation>()
                 .WithMany()
                 .HasForeignKey(x => x.designationId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
