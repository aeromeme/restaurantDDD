using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LineItem
    {
        public LineItemId Id { get; private set; }
        public ProductId ProductId { get; private set;}
        public int Quantity { get; private set; }
        public Money Price { get; private set; } = null!;

        public readonly Product? Product;
        public LineItem(ProductId productId, int quantity, Money price, LineItemId? itemId=null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (price.Amount <= 0)
                throw new DomainException("Price must be positive.");
            if (string.IsNullOrEmpty(price.Currency))
                throw new DomainException("Currency is required.");
            Id = itemId ?? LineItemId.NewId();
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
        private LineItem()
        {
        }
        // --- State changing operations ---
        public void ChangeQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            Quantity = quantity;
        }
        public void ChangePrice(Money newPrice)
        {
            if (newPrice.Amount <= 0)
                throw new DomainException("Price must be positive.");
            if (string.IsNullOrEmpty(newPrice.Currency))
                throw new DomainException("Currency is required.");
            Price = newPrice;
        }
    }
    public readonly record struct LineItemId(Guid Value)
    {
        public static LineItemId NewId() => new LineItemId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
