using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id.Value,
                Name = customer.Name,
                Email = customer.Email
            };
        }
    }
}