using Microsoft.EntityFrameworkCore;
using pharmacy_management.Entities;

namespace pharmacy_management.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
