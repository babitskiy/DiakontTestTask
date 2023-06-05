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
            //Database.EnsureDeleted();
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
                    new Department { Id = 2, Name = "ТО" }
            );
            modelBuilder.Entity<Position>().HasData(
                    new Position { Id = 1, Name = "Инженер-конструктор" },
                    new Position { Id = 2, Name = "Инженер-технолог" }
            );
            modelBuilder.Entity<Rate>().HasData(
                    new Rate { Id = 1, Salary = 10000, StartDate = new DateTime(2015, 1, 1), PositionId = 1 },
                    new Rate { Id = 2, Salary = 15000, StartDate = new DateTime(2015, 6, 1), PositionId = 1 },
                    new Rate { Id = 3, Salary = 12000, StartDate = new DateTime(2015, 1, 1), PositionId = 2 },
                    new Rate { Id = 4, Salary = 17000, StartDate = new DateTime(2015, 6, 1), PositionId = 2 }
            );
            modelBuilder.Entity<StaffingTableElement>().HasData(
                    new StaffingTableElement { Id = 1, StartDate = new DateTime(2015, 1, 1), PositionId = 1, DepartmentId = 1, EmployeesCount = 5 },
                    new StaffingTableElement { Id = 2, StartDate = new DateTime(2015, 6, 1), PositionId = 1, DepartmentId = 1, EmployeesCount = 10 },
                    new StaffingTableElement { Id = 3, StartDate = new DateTime(2015, 1, 1), PositionId = 2, DepartmentId = 2, EmployeesCount = 5 },
                    new StaffingTableElement { Id = 4, StartDate = new DateTime(2015, 6, 1), PositionId = 2, DepartmentId = 2, EmployeesCount = 10 }
            );
        }
    }
}
