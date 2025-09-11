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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RestaurantDbContext _restaurantDbContext;
        public CategoryRepository(RestaurantDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }
        public  async Task<IReadOnlyList<Category>> GetAllAsync()
        {
           return await _restaurantDbContext.Categories.ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(CategoryId id)
        {
            return await _restaurantDbContext.Categories.FindAsync(id);
        }
    }
}
