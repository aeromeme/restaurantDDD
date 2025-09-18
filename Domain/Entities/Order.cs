using Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class Order
    {
        private readonly HashSet<LineItem> _lineItems = new();
        public OrderId Id { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public IReadOnlyCollection<LineItem> LineItems => _lineItems.ToList().AsReadOnly();
        public DateTime OrderDate { get; private set; }
        public readonly Customer Customer = null!;
        public string Status { get; private set; } = OrderStatus.Pending;

        public Order(CustomerId customerId)
        {
            Id = OrderId.NewId();
            CustomerId = customerId;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending;
        }
        private Order() { }

        private Result EnsureOrderIsModifiable()
        {
            if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
                return Result.Fail("No changes allowed for completed or cancelled orders.");
            return Result.Ok();
        }

        public Result ChangeCustomer(CustomerId customerId)
        {
            var modifiable = EnsureOrderIsModifiable();
            if (!modifiable.Success) return modifiable;
            CustomerId = customerId;
            return Result.Ok();
        }

        public Result AddLine(LineItem item)
        {
            var modifiable = EnsureOrderIsModifiable();
            if (!modifiable.Success) return modifiable;
            if (item == null)
                return Result.Fail("LineItem is required.");

            var existing = _lineItems.FirstOrDefault(li => li.ProductId == item.ProductId);
            if (existing != null)
            {
                var quantityResult = existing.ChangeQuantity(existing.Quantity + item.Quantity);
                if (!quantityResult.Success) return quantityResult;
                var priceResult = existing.ChangePrice(item.Price);
                if (!priceResult.Success) return priceResult;
                return Result.Ok("Quantity updated for existing line item.");
            }

            _lineItems.Add(item);
            return Result.Ok("Line item added.");
        }

        public Result RemoveLine(LineItem item)
        {
            var modifiable = EnsureOrderIsModifiable();
            if (!modifiable.Success) return modifiable;
            if (item == null)
                return Result.Fail("LineItem is required.");
            if (!_lineItems.Remove(item))
                return Result.Fail("LineItem not found.");
            return Result.Ok("Line item removed.");
        }

        public Result UpdateLine(LineItem updatedItem)
        {
            var modifiable = EnsureOrderIsModifiable();
            if (!modifiable.Success) return modifiable;
            var existing = _lineItems.FirstOrDefault(li => li.Id == updatedItem.Id);
            if (existing == null)
                return Result.Fail("LineItem not found.");

            var quantityResult = existing.ChangeQuantity(updatedItem.Quantity);
            if (!quantityResult.Success) return quantityResult;
            var priceResult = existing.ChangePrice(updatedItem.Price);
            if (!priceResult.Success) return priceResult;
            return Result.Ok("Line item updated.");
        }

        public Result ChangeStatus(string status)
        {
            if (!OrderStatus.All.Contains(status))
                return Result.Fail($"Invalid status: {status}");

            var modifiable = EnsureOrderIsModifiable();
            if (!modifiable.Success) return modifiable;

            if (status == OrderStatus.Completed && !_lineItems.Any())
                return Result.Fail("Cannot complete an order with no line items.");

            Status = status;
            return Result.Ok($"Order status changed to {status}.");
        }
    }

    public readonly record struct OrderId(Guid Value)
    {
        public static OrderId NewId() => new OrderId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public static readonly HashSet<string> All = new() { Pending, Completed, Cancelled };
    }
}
