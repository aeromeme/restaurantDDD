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

        public DbSet<Customer> Customer { get; set; } = null!;

        public DbSet<Order> Order { get; set; } = null!;

        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
   
            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CategoryId(value));    

            modelBuilder.Entity<Product>(entity =>
            {
                entity.OwnsOne(p => p.Price, mv =>
                {
                    mv.Property(m => m.Amount)
                        .HasColumnName("PriceAmount")
                        .HasColumnType("decimal(18,2)"); // Explicit precision and scale
                    mv.Property(m => m.Currency).HasColumnName("PriceCurrency");
                });
                entity.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey("CategoryId");

                entity.Property(p => p.RowVersion)
                .IsRowVersion();

                entity.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value));
            });
              
                 // Use shadow property or add CategoryId to Product

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.Id)
                    .HasConversion(
                        id => id.Value,
                        value => new CustomerId(value))
                    ;
                entity.Property(c=> c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(250);

                entity.HasIndex(c => c.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(c => c.Id)
                  .HasConversion(
                      id => id.Value,
                      value => new OrderId(value));
                entity.HasOne(c=> c.Customer)
                    .WithMany()
                    .HasForeignKey(o=>o.CustomerId)
                    .IsRequired();

                entity.HasMany(o => o.LineItems)
                    .WithOne()
                    .HasForeignKey("OrderId");


            });

            modelBuilder.Entity<LineItem>(entity =>
            {
                entity.Property(c => c.Id)
                  .HasConversion(
                      id => id.Value,
                      value => new LineItemId(value));
                entity.OwnsOne(li => li.Price, mv =>
                {
                    mv.Property(m => m.Amount)
                        .HasColumnName("LineItemPriceAmount")
                        .HasColumnType("decimal(18,2)"); // Explicit precision and scale
                    mv.Property(m => m.Currency).HasColumnName("LineItemPriceCurrency");
                });
                entity.HasOne(p=> p.Product)
                    .WithMany()
                    .HasForeignKey(li=>li.ProductId)
                    .IsRequired();
                entity.Property(li => li.Quantity)
                    .IsRequired();
            });






            base.OnModelCreating(modelBuilder);
        }
    }
}