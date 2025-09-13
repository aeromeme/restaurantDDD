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
            return await _restaurantDbContext.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(ProductId id)
        {
            // FindAsync does not support Include, so use FirstOrDefaultAsync with a predicate
            return await _restaurantDbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<(int Count, IReadOnlyList<Product>)> GetPaged(string? searchTerm, string? sortBy, int pageNumber, int pageSize) {

            var query = GetQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Category.Name.Contains(searchTerm));
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "name" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "price" => query.OrderBy(p => p.Price.Amount),
                "price_desc" => query.OrderByDescending(p => p.Price.Amount),
                "category" => query.OrderBy(p => p.Category.Name),
                "category_desc" => query.OrderByDescending(p => p.Category.Name),
                _ => query.OrderBy(p => p.Name)
            };

            // Pagination (use async EF Core methods)
            var totalItems = await query.CountAsync();
            var products = await query
                .Include(p => p.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category) // Ensure Category is included
                .ToListAsync();

            return (totalItems, products);

        }


    }
}
