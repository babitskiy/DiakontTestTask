using Microsoft.EntityFrameworkCore;

namespace DiakontTestTask.Models.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DiakontDB;Trusted_Connection=True;");
        }
    }
}
