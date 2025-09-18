using Domain.Exceptions;
using Domain.ValueObjects;
using Domain.Results;
using System;

namespace Domain.Entities
{
    public class LineItem
    {
        public LineItemId Id { get; private set; }
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Money Price { get; private set; } = null!;
        public readonly Product? Product;

        private LineItem(ProductId productId, int quantity, Money price, LineItemId? itemId = null)
        {
            Id = itemId ?? LineItemId.NewId();
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public static Result<LineItem> Create(ProductId productId, int quantity, Money price, LineItemId? itemId = null)
        {
            if (quantity <= 0)
                return Result<LineItem>.Fail("Quantity must be greater than zero.");
            if (price == null || price.Amount <= 0)
                return Result<LineItem>.Fail("Price must be positive.");
            if (price != null && string.IsNullOrEmpty(price.Currency))
                return Result<LineItem>.Fail("Currency is required.");

            var item = new LineItem(productId, quantity, price, itemId);
            return Result<LineItem>.Ok(item);
        }

        public Result ChangeQuantity(int quantity)
        {
            if (quantity <= 0)
                return Result.Fail("Quantity must be greater than zero.");
            Quantity = quantity;
            return Result.Ok();
        }

        public Result ChangePrice(Money newPrice)
        {
            if (newPrice == null || newPrice.Amount <= 0)
                return Result.Fail("Price must be positive.");
            if (newPrice != null && string.IsNullOrEmpty(newPrice.Currency))
                return Result.Fail("Currency is required.");
            Price = newPrice;
            return Result.Ok();
        }
    }

    public readonly record struct LineItemId(Guid Value)
    {
        public static LineItemId NewId() => new LineItemId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
