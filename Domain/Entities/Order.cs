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

        public Order(CustomerId customerId)
        {
            Id = OrderId.NewId();
            CustomerId = customerId;
            OrderDate = DateTime.UtcNow;
        }
        private Order()
        {
        }

        public bool addLine(LineItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "LineItem is required.");
            _lineItems.Add(item);
            return true;
        }
        public bool removeLine(LineItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "LineItem is required.");
            return _lineItems.Remove(item);
        }
    }
    public readonly record struct OrderId(Guid Value)
    {
        public static OrderId NewId() => new OrderId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
