using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        public DbSet<Production> Productions { get; set; }
        public DbSet<Machine> Machines { get; set; }
    }
}
