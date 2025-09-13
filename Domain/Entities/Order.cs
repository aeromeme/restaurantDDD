using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void EnsureOrderIsModifiable()
        {
            if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("No changes allowed for completed or cancelled orders.");
        }

        public void changeCustomer(CustomerId customerId)
        {
            EnsureOrderIsModifiable();
            CustomerId = customerId;
        }

        public bool addLine(LineItem item)
        {
            EnsureOrderIsModifiable();
            if (item == null)
                throw new ArgumentNullException(nameof(item), "LineItem is required.");

            var existing = _lineItems.FirstOrDefault(li => li.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.ChangeQuantity(existing.Quantity + item.Quantity);
                existing.ChangePrice(item.Price);
                return false;
            }

            _lineItems.Add(item);
            return true;
        }

        public bool removeLine(LineItem item)
        {
            EnsureOrderIsModifiable();
            if (item == null)
                throw new ArgumentNullException(nameof(item), "LineItem is required.");
            return _lineItems.Remove(item);
        }

        public bool UpdateLine(LineItem updatedItem)
        {
            EnsureOrderIsModifiable();
            var existing = _lineItems.FirstOrDefault(li => li.Id == updatedItem.Id);
            if (existing == null)
                return false;

            existing.ChangeQuantity(updatedItem.Quantity);
            existing.ChangePrice(updatedItem.Price);
            return true;
        }

        public void ChangeStatus(string status)
        {
            if (!OrderStatus.All.Contains(status))
                throw new ArgumentException($"Invalid status: {status}");
            EnsureOrderIsModifiable();
            // Prevent completing an order with no line items
            if (status == OrderStatus.Completed && !_lineItems.Any())
                throw new InvalidOperationException("Cannot complete an order with no line items.");

            Status = status;
        }
    }
    public readonly record struct OrderId(Guid Value)
    {
        public static OrderId NewId() => new OrderId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    // Static class for predefined status values
    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public static readonly HashSet<string> All = new() { Pending, Completed, Cancelled };
    }
}
