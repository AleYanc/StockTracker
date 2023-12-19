using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockTracker.Models;

namespace StockTracker.Database
{
    public class DatabaseConnection(DbContextOptions options) : IdentityDbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<ProductCode> ProductCodes { get; set; }
        public DbSet<ImportHistory> ImportHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCode>()
                .Property(x => x.CreatedDate)
                .HasDefaultValue(DateTime.Now);
            modelBuilder.Entity<ImportHistory>()
                .Property(x => x.ImportDate)
                .HasDefaultValue(DateTime.Now);

            base.OnModelCreating(modelBuilder);
        }
    }
}
