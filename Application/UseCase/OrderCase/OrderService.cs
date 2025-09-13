using Application.DTOs;
using Application.Mappers;
using Domain.Entities;
using Domain.Ports;
using Domain.ValueObjects;
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
                    throw new ApplicationException($"Product with ID {itemDto.ProductId} not found.");

                // Use product.Price instead of itemDto.Price
                var lineItem = new LineItem(
                    productId,
                    itemDto.Quantity,
                    product.Price
                );

                order.addLine(lineItem);
            }

            _orderRepository.Add(order);
            await _unitOfWork.SavesChangesAsync();

            order = await _orderRepository.GetByIdAsync(order.Id);
            if (order == null)
                throw new ApplicationException("Error retrieving the created order.");
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
                existingOrder.changeCustomer(new CustomerId(orderDto.CustomerId));
            }

            var incomingLineIds = orderDto.LineItems.Select(li => li.LineItemId).ToHashSet();
            var toDelete = existingOrder.LineItems.Where(li => !incomingLineIds.Contains(li.Id.Value)).ToList();
            foreach (var item in toDelete)
                existingOrder.removeLine(item);

            foreach (var itemDto in orderDto.LineItems)
            {
                var productId = new ProductId(itemDto.ProductId);
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    throw new ApplicationException($"Product with ID {itemDto.ProductId} not found.");

                // Use product.Price instead of itemDto.Price
                var lineItem = new LineItem(
                    productId,
                    itemDto.Quantity,
                    product.Price,
                    new LineItemId(itemDto.LineItemId)
                );

                if (!existingOrder.UpdateLine(lineItem))
                {
                    existingOrder.addLine(lineItem);
                }
            }

            _orderRepository.Update(existingOrder);
            await _unitOfWork.SavesChangesAsync();

            existingOrder = await _orderRepository.GetByIdAsync(existingOrder.Id);
            if (existingOrder == null)
                throw new ApplicationException("Error retrieving the updated order.");
            var resultDto = OrderMapper.ToDto(existingOrder);
            return Result<OrderDto>.Ok(resultDto);
        }
        public async Task<IResult<OrderDto>> CompleteOrderAsync(Guid Id)
        {
            var orderId = new OrderId(Id);
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result<OrderDto>.Fail("Order not found.");

            try
            {
                order.ChangeStatus(OrderStatus.Completed);
            }
            catch (Exception ex)
            {
                return Result<OrderDto>.Fail(ex.Message);
            }

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

            try
            {
                order.ChangeStatus(OrderStatus.Cancelled);
            }
            catch (Exception ex)
            {
                return Result<OrderDto>.Fail(ex.Message);
            }

            _orderRepository.Update(order);
            await _unitOfWork.SavesChangesAsync();

            var resultDto = OrderMapper.ToDto(order);
            return Result<OrderDto>.Ok(resultDto);
        }
    }
}
