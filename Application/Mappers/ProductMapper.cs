using Application.DTOs;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.Id.Value,
                Name = product.Name,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                Stock = product.Stock,
                IsActive = product.IsActive,
                Category = CategoryMapper.ToDto(product.Category)
            };
        }
    }
}