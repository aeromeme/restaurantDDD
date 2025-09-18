using Microsoft.AspNetCore.Mvc;
using Application.UseCase.OrderCase;
using Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Results;

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
        [ProducesResponseType(typeof(Result<IReadOnlyList<OrderDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var result = await _orderService.GetAll();
            if (!result.Success)
                return BadRequest(result);
            if (result.Data == null)
                return BadRequest(Result<IReadOnlyList<OrderDto>>.Fail("No orders found."));
            return Ok(result.Data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] OrderCreateDto dto)
        {
            var result = await _orderService.AddOrderAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            if (result.Data == null)
                return BadRequest(Result<OrderDto>.Fail("Order creation failed."));
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, OrderUpdateDto dto)
        {
            if (id != dto.OrderId)
                return BadRequest(Result<OrderDto>.Fail("Order ID mismatch."));
            var result = await _orderService.UpdateOrderAsync(dto);
            if (!result.Success)
            {
                if (result.Message != null && result.Message.ToLower().Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }
            if (result.Data == null)
                return BadRequest(Result<OrderDto>.Fail("Order update failed."));
            return Ok(result.Data);
        }

        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            var result = await _orderService.CompleteOrderAsync(id);
            if (!result.Success)
            {
                if (result.Message != null && result.Message.ToLower().Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }
            if (result.Data == null)
                return BadRequest(Result<OrderDto>.Fail("Order completion failed."));
            return Ok(result.Data);
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.Success)
            {
                if (result.Message != null && result.Message.ToLower().Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }
            if (result.Data == null)
                return BadRequest(Result<OrderDto>.Fail("Order cancellation failed."));
            return Ok(result.Data);
        }
    }
}
