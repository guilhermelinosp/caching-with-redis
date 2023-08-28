using Microsoft.EntityFrameworkCore;
using Sample.API.Models;

namespace Sample.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Driver>? Drivers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
