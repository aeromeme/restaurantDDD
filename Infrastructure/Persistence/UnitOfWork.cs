using Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantDbContext _restaurantDbContext;
        public UnitOfWork(RestaurantDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }
        public async Task SavesChangesAsync()
        {
            await _restaurantDbContext.SaveChangesAsync();
        }
    }
}
