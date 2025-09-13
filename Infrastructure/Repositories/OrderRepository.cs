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
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly RestaurantDbContext _restaurantDbContext;
        public OrderRepository(RestaurantDbContext restaurantDbContext) : base(restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }
        public async Task<IReadOnlyList<Order>> GetAllAsync()
        {
            return await _restaurantDbContext.Order
                .Include(p => p.Customer)
                .Include(p => p.LineItems)
                .ThenInclude(p => p.Product)
                 .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(OrderId id)
        {
            // FindAsync does not support Include, so use FirstOrDefaultAsync with a predicate
            return await _restaurantDbContext.Order
                .Include(p => p.Customer)
                .Include(p => p.LineItems)
                .ThenInclude(p=> p.Product)
                .ThenInclude(p=> p.Category)    
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<(int Count, IReadOnlyList<Order>)> GetPaged(string? searchTerm, string? sortBy, int pageNumber, int pageSize)
        {
            var query = GetQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Fix CS1061: Remove reference to p.Category
                // Fix CS8602: Use null-conditional operator for p.Customer
                query = query.Where(p => p.Customer != null && p.Customer.Name.Contains(searchTerm));
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "customer" => query.OrderBy(p => p.Customer.Name), // This may need fixing if Order does not have Name property
                "customer_desc" => query.OrderByDescending(p => p.Customer.Name),
                "orderdate" => query.OrderBy(p => p.OrderDate),
                "orderdate_desc" => query.OrderByDescending(p => p.OrderDate),
                // Remove category sorting since Order does not have Category
                _ => query.OrderByDescending(p => p.OrderDate)
            };

            // Pagination (use async EF Core methods)
            var totalItems = await query.CountAsync();
            var products = await query
                .Include(p => p.Customer)
                .Include(p => p.LineItems)
                .ThenInclude(p => p.Product)
                 .ThenInclude(p => p.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, products);
        }
    }
}
