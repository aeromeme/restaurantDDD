using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
    {
        public RestaurantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost\\manusql;Database=restaurantDDD;Trusted_Connection=True;TrustServerCertificate=True");

            return new RestaurantDbContext(optionsBuilder.Options);
        }
    }
}