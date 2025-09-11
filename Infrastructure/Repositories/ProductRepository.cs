using Domain.Entities;
using Domain.Ports;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductRepository:RepositoryBase<Product>,IProductRepository
    {
        private readonly RestaurantDbContext _restaurantDbContext;
        public ProductRepository(RestaurantDbContext restaurantDbContext):base(restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }
        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            return await _restaurantDbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(ProductId id)
        {
            return await _restaurantDbContext.Products.FindAsync(id);
        }
       

    }
}
