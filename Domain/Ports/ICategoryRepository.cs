using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync();

        Task<Category> GetByIdAsync(CategoryId id);
    }
}
