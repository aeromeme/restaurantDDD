using Application.DTOs;
using Application.Mappers;
using Domain.Entities;
using Domain.Ports;
using Domain.ValueObjects;
using Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.OrderCase
{
    public class OrderService
    {
        public ICategoryRepository _categoryRepository;
        public IProductRepository _productRepository;
        public IOrderRepository _orderRepository;
        public IUnitOfWork _unitOfWork;
        public OrderService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IResult<IReadOnlyList<OrderDto>>> GetAll()
        {
            var data = await _orderRepository.GetAllAsync();
            var orders = data.Select(OrderMapper.ToDto).ToList();
            return Result<IReadOnlyList<OrderDto>>.Ok(orders);
        }

        public async Task<IResult<OrderDto>> AddOrderAsync(OrderCreateDto orderDto)
        {
            var customerId = new CustomerId(orderDto.CustomerId);
            var order = new Order(customerId);

            foreach (var itemDto in orderDto.LineItems)
            {
                var productId = new ProductId(itemDto.ProductId);
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return Result<OrderDto>.Fail($"Product with ID {itemDto.ProductId} not found.");

                var lineItemResult = LineItem.Create(productId, itemDto.Quantity, product.Price);
                if (!lineItemResult.Success || lineItemResult.Data == null)
                    return Result<OrderDto>.Fail(lineItemResult.Message ?? "Invalid line item.");

                var addResult = order.AddLine(lineItemResult.Data);
                if (!addResult.Success)
                    return Result<OrderDto>.Fail(addResult.Message ?? "Failed to add line item.");
            }

            _orderRepository.Add(order);
            await _unitOfWork.SavesChangesAsync();

            order = await _orderRepository.GetByIdAsync(order.Id);
            if (order == null)
                return Result<OrderDto>.Fail("Error retrieving the created order.");
            var resultDto = OrderMapper.ToDto(order);
            return Result<OrderDto>.Ok(resultDto);
        }

        public async Task<IResult<OrderDto>> GetById(OrderId id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");
            var orderDto = OrderMapper.ToDto(order);
            return Result<OrderDto>.Ok(orderDto);
        }

        public async Task<IResult<PagedOrderListDto>> GetPaged(string? searchTerm, string? sortBy, int pageNumber, int pageSize)
        {
            var (totalCount, orders) = await _orderRepository.GetPaged(searchTerm, sortBy, pageNumber, pageSize);
            var orderDtos = orders.Select(OrderMapper.ToDto).ToList();
            var pagedResult = new PagedOrderListDto
            {
                TotalCount = totalCount,
                Orders = orderDtos
            };
            return Result<PagedOrderListDto>.Ok(pagedResult);
        }

        public async Task<IResult> DeleteOrderAsync(OrderId id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return Result.Fail("Order not found.");
            _orderRepository.Delete(order);
            await _unitOfWork.SavesChangesAsync();
            return Result.Ok();
        }

        public async Task<IResult<OrderDto>> UpdateOrderAsync(OrderUpdateDto orderDto)
        {
            var orderId = new OrderId(orderDto.OrderId);
            var existingOrder = await _orderRepository.GetByIdAsync(orderId);
            if (existingOrder == null)
                return Result<OrderDto>.Fail("Order not found.");

            if (existingOrder.CustomerId.Value != orderDto.CustomerId)
            {
                var customerResult = existingOrder.ChangeCustomer(new CustomerId(orderDto.CustomerId));
                if (!customerResult.Success)
                    return Result<OrderDto>.Fail(customerResult.Message ?? "Failed to change customer.");
            }

            var incomingLineIds = orderDto.LineItems.Select(li => li.LineItemId).ToHashSet();
            var toDelete = existingOrder.LineItems.Where(li => !incomingLineIds.Contains(li.Id.Value)).ToList();
            foreach (var item in toDelete)
            {
                var removeResult = existingOrder.RemoveLine(item);
                if (!removeResult.Success)
                    return Result<OrderDto>.Fail(removeResult.Message ?? "Failed to remove line item.");
            }

            foreach (var itemDto in orderDto.LineItems)
            {
                var productId = new ProductId(itemDto.ProductId);
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return Result<OrderDto>.Fail($"Product with ID {itemDto.ProductId} not found.");

                var lineItemResult = LineItem.Create(productId, itemDto.Quantity, product.Price, new LineItemId(itemDto.LineItemId));
                if (!lineItemResult.Success || lineItemResult.Data == null)
                    return Result<OrderDto>.Fail(lineItemResult.Message ?? "Invalid line item.");

                var updateResult = existingOrder.UpdateLine(lineItemResult.Data);
                if (!updateResult.Success)
                {
                    var addResult = existingOrder.AddLine(lineItemResult.Data);
                    if (!addResult.Success)
                        return Result<OrderDto>.Fail(addResult.Message ?? "Failed to add line item.");
                }
            }

            _orderRepository.Update(existingOrder);
            await _unitOfWork.SavesChangesAsync();

            existingOrder = await _orderRepository.GetByIdAsync(existingOrder.Id);
            if (existingOrder == null)
                return Result<OrderDto>.Fail("Error retrieving the updated order.");
            var resultDto = OrderMapper.ToDto(existingOrder);
            return Result<OrderDto>.Ok(resultDto);
        }

        public async Task<IResult<OrderDto>> CompleteOrderAsync(Guid Id)
        {
            var orderId = new OrderId(Id);
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var statusResult = order.ChangeStatus(OrderStatus.Completed);
            if (!statusResult.Success)
                return Result<OrderDto>.Fail(statusResult.Message ?? "Failed to complete order.");

            _orderRepository.Update(order);
            await _unitOfWork.SavesChangesAsync();

            var resultDto = OrderMapper.ToDto(order);
            return Result<OrderDto>.Ok(resultDto);
        }

        public async Task<IResult<OrderDto>> CancelOrderAsync(Guid Id)
        {
            var orderId = new OrderId(Id);
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            var statusResult = order.ChangeStatus(OrderStatus.Cancelled);
            if (!statusResult.Success)
                return Result<OrderDto>.Fail(statusResult.Message ?? "Failed to cancel order.");

            _orderRepository.Update(order);
            await _unitOfWork.SavesChangesAsync();

            var resultDto = OrderMapper.ToDto(order);
            return Result<OrderDto>.Ok(resultDto);
        }
    }
}
