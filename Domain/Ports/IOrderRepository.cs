using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IOrderRepository
    {
        void Add(Order order);
        void Update(Order order);
        void Delete(Order order);
        Task<Order?> GetByIdAsync(OrderId id);
        Task<IReadOnlyList<Order>> GetAllAsync();

        Task<(int Count, IReadOnlyList<Order>)> GetPaged(string? searchTerm, string? sortBy, int pageNumber, int pageSize);
    }
}
