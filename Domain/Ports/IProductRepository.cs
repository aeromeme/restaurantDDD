using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(ProductId id);
        Task<Product?> GetByIdAsync(ProductId id);
        Task<IReadOnlyList<Product>> GetAllAsync();
    }
}
