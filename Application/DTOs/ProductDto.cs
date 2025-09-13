using Domain.Entities;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class ProductDto : ProductDtoBase
    {
        public bool IsActive { get; set; }

        public Guid ProductId { get; set; }

        public Money Price { get; set; } = null!;

        public CategoryDto Category { get; set; } = null!;

    }
    public class ProductCreateDto : ProductDtoBase
    {
        public Guid CategoryId { get; set; }

        public decimal Price { get; set; }
    }
    public class ProductUpdateDto : ProductDtoBase
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }

        public decimal Price { get; set; }
    }
    public abstract class ProductDtoBase
    {

        
        public string Name { get; set; } = string.Empty;
     

        public int Stock { get; set; }

    }
}