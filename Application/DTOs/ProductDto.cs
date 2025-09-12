using Domain.Entities;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class ProductDto : ProductDtoBase
    {
        public bool IsActive { get; set; }

        public Guid ProductId { get; set; }

        public CategoryDto Category { get; set; } = null!;

    }
    public class ProductCreateDto : ProductDtoBase
    {
        public Guid CategoryId { get; set; }
    }
    public class ProductUpdateDto : ProductDtoBase
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
    public class ProductDtoBase
    {

        [JsonIgnore] // Hides from JSON serialization and Swagger
        public string Currency { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int Stock { get; set; }

    }
}