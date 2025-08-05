using Microsoft.EntityFrameworkCore;
using pharmacy_management.Entities;

namespace pharmacy_management.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Drug> Drugs { get; set; }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        // drug price 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Drug>()
                .Property(d => d.Price)
                .HasPrecision(18, 2); // 18 total digits, 2 after the decimal point

            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.PricePerUnit)
                .HasPrecision(18, 2);
        }
    }
}
