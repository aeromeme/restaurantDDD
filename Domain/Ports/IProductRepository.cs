using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IProductRepository
    {
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        Task<Product?> GetByIdAsync(ProductId id);
        Task<IReadOnlyList<Product>> GetAllAsync();

        Task<(int Count,IReadOnlyList<Product>)> GetPaged(string? searchTerm, string? sortBy, int pageNumber, int pageSize);
    }
}
