using Domain.Entities;
using Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.CategoryCase
{
    public class GetAllCategories 
    {
        public ICategoryRepository _categoryRepository;
        public GetAllCategories(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<IReadOnlyList<Category>> ExecuteAsync()
        {
            var data = _categoryRepository.GetAllAsync();
            return data;
        }
    }
}
