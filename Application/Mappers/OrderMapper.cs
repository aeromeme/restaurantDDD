using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.Id.Value,
                Customer = CustomerMapper.ToDto(order.Customer),
                LineItems = order.LineItems.Select(item => new LineItemDto
                {
                    LineItemId = item.Id.Value,
                    Product = item.Product!=null ? ProductMapper.ToDto(item.Product): null!,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList(),
                OrderDate = order.OrderDate
            };
        }
    }
}