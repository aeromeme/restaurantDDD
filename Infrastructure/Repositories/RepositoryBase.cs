using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public abstract class RepositoryBase<TModel> where TModel : class
    {
        private readonly RestaurantDbContext _restaurantDbContext;

        public RepositoryBase(RestaurantDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }

        public void Add(TModel item)
        {
            _restaurantDbContext.Set<TModel>().Add(item);
        }

        public IQueryable<TModel> GetQueryable()
        {
            return _restaurantDbContext.Set<TModel>().AsNoTracking();
        }

        public void Update(TModel item)
        {
            _restaurantDbContext.Set<TModel>().Update(item);
        }

        public void Delete(TModel item)
        {
            _restaurantDbContext.Set<TModel>().Remove(item);
        }
    }
}