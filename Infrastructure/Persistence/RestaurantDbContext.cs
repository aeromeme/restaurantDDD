using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infrastructure.Persistence
{
    public class RestaurantDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ProductId and CategoryId as value objects
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value));

            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CategoryId(value));

            // RowVersion for concurrency
            modelBuilder.Entity<Product>()
                .Property(p => p.RowVersion)
                .IsRowVersion();

            // Money value object conversion
            modelBuilder.Entity<Product>()
                .OwnsOne(p => p.Price, mv =>
                {
                    mv.Property(m => m.Amount)
                        .HasColumnName("PriceAmount")
                        .HasColumnType("decimal(18,2)"); // Explicit precision and scale
                    mv.Property(m => m.Currency).HasColumnName("PriceCurrency");
                });


            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey("CategoryId"); // Use shadow property or add CategoryId to Product

            base.OnModelCreating(modelBuilder);
        }
    }
}