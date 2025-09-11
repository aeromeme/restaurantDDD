using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class CategoryMapper
    {
        public static DTOs.CategoryDto ToDto(Domain.Entities.Category category)
        {
            return new DTOs.CategoryDto
            {
                Id = category.Id.Value,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
