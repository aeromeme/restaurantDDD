using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface ICustomerRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync();

        Task<Category> GetByIdAsync(CategoryId id);

        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);

        Task<(int Count, IReadOnlyList<Category>)> GetPaged(string? searchTerm, string? sortBy, int pageNumber, int pageSize);
    }
}
