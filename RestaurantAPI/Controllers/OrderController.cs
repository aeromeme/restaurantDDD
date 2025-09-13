using Microsoft.AspNetCore.Mvc;
using Application.UseCase.OrderCase;
using Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using Application.DTOs;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetAll();
            return Ok(orders.Data);
        }
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] OrderCreateDto dto)
        {
            var result = await _orderService.AddOrderAsync(dto);
            return Ok(result.Data);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, OrderUpdateDto dto)
        {
            if (id != dto.OrderId)
                return BadRequest("Order ID mismatch.");
            // Assuming you have an UpdateOrderAsync method in your OrderService
            var result = await _orderService.UpdateOrderAsync(dto);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            var result = await _orderService.CompleteOrderAsync(id);
            if (!result.Success)
            {
                if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result.Message);
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.Success)
            {
                if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result.Message);
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
