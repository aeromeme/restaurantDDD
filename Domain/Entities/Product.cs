using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;
using Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class Product
    {
        public ProductId Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Money Price { get; private set; } = null!;
        public int Stock { get; private set; }
        public bool IsActive { get; private set; }
        public Category Category { get; private set; } = null!;
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        private Product(string name, Money price, int stock, Category category)
        {
            Id = ProductId.NewId();
            Name = name;
            Price = price;
            Stock = stock;
            IsActive = true;
            Category = category;
        }

        public static Result<Product> Create(string name, Money price, int stock, Category category)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Product>.Fail("Product name is required.");
            if (stock < 0)
                return Result<Product>.Fail("Initial stock cannot be negative.");
            if (price == null || price.Amount <= 0)
                return Result<Product>.Fail("Price must be positive.");
            if (category == null)
                return Result<Product>.Fail("Category is required.");
            if (price != null && string.IsNullOrEmpty(price.Currency))
                return Result<Product>.Fail("Currency is required.");

            var product = new Product(name, price, stock, category);
            return Result<Product>.Ok(product);
        }

        // --- State changing operations ---
        public Result ChangeCategory(Category category)
        {
            if (category == null)
                return Result.Fail("Category is required.");
            Category = category;
            return Result.Ok();
        }

        public Result ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Fail("Product name is required.");
            Name = name;
            return Result.Ok();
        }

        public Result ChangePrice(Money newPrice)
        {
            if (newPrice == null || newPrice.Amount <= 0)
                return Result.Fail("Price must be positive.");
            if (string.IsNullOrEmpty(newPrice.Currency))
                return Result.Fail("Currency is required.");
            Price = newPrice;
            return Result.Ok();
        }

        public Result IncreaseStock(int qty)
        {
            if (qty <= 0)
                return Result.Fail("Quantity must be positive.");
            Stock += qty;
            return Result.Ok();
        }

        public Result ReduceStock(int qty)
        {
            if (qty <= 0)
                return Result.Fail("Quantity must be positive.");
            if (Stock < qty)
                return Result.Fail("Not enough stock.");
            Stock -= qty;

            if (Stock == 0)
                AddDomainEvent(new ProductOutOfStock(Id.Value));
            return Result.Ok();
        }

        // --- Validation ---
        public bool IsAvailable(int qty) => Stock >= qty && IsActive;

        // --- Workflow ---
        public Result Deactivate()
        {
            if (Stock > 0)
                return Result.Fail("Cannot deactivate a product with stock.");
            IsActive = false;
            return Result.Ok();
        }

        // Domain events pattern (simplified)
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();
        private void AddDomainEvent(IDomainEvent e) => _events.Add(e);
    }

    public readonly record struct ProductId(Guid Value)
    {
        public static ProductId NewId() => new ProductId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
