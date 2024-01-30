using Microsoft.EntityFrameworkCore;
using WebApplicationTraining.Models;

namespace WebApi.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        public DataContext() { }


        public DbSet<Production> Productions { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=FinalEs;Persist Security Info=True;User ID=sa;Password=Uform@2023#;Encrypt=False");
        }
    }
}
