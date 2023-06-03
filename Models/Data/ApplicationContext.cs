using Microsoft.EntityFrameworkCore;
using System;

namespace DiakontTestTask.Models.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StaffingTableElement> StaffingTableElements { get; set; }

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DiakontDB;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasData(
                    new Department { Id = 1, Name = "КБ" },
                    new Department { Id = 2, Name = "Технологический отдел" }
            );
            modelBuilder.Entity<Position>().HasData(
                    new Position { Id = 1, Name = "Инженер-конструктор" },
                    new Position { Id = 2, Name = "Инженер-технолог" }
            );
            modelBuilder.Entity<Rate>().HasData(
                    new Rate { Id = 1, Salary = 10000, StartDate = new DateTime(2023, 5, 15), PositionId = 1 },
                    new Rate { Id = 2, Salary = 20000, StartDate = new DateTime(2023, 6, 2), PositionId = 2 }
            );
            modelBuilder.Entity<StaffingTableElement>().HasData(
                    new StaffingTableElement { Id = 1, StartDate = new DateTime(2023, 5, 15), PositionId = 1, DepartmentId = 1 },
                    new StaffingTableElement { Id = 2, StartDate = new DateTime(2023, 6, 2), PositionId = 2, DepartmentId = 2 }
            );
        }
    }
}
