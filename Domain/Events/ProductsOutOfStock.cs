using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class ProductOutOfStock : IDomainEvent
    {
        public Guid ProductId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public ProductOutOfStock(Guid productId)
        {
            ProductId = productId;
        }
    }
}
