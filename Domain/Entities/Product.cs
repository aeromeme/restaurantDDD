using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    public class Product
    {
        public ProductId Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Money Price { get; private set; } = null!;
        public int Stock { get; private set; }
        public bool IsActive { get; private set; }
        public Category Category { get; private set; } = null!; // Add this line

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public Product(string name, Money price, int stock, Category category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name is required.");
            if (stock < 0)
                throw new DomainException("Initial stock cannot be negative.");
            if (price.Amount <= 0)
                throw new DomainException("Price must be positive.");
            if (category == null)
                throw new DomainException("Category is required.");
            if (string.IsNullOrEmpty(price.Currency))
                throw new DomainException("Currency is required.");


            Id = ProductId.NewId();
            Name = name;
            Price = price;
            Stock = stock;
            IsActive = true;
            Category = category; // Initialize the Category property
        }

        private Product()
        {
        }

        // --- State changing operations ---
        public void ChangeCategory(Category category)
        {
            if (category == null)
                throw new DomainException("Category is required.");
            Category = category;
        }
        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name is required.");
            Name = name;
        }
        public void ChangePrice(Money newPrice)
        {
            if (newPrice.Amount <= 0)
                throw new DomainException("Price must be positive.");
            if (string.IsNullOrEmpty(newPrice.Currency))
                throw new DomainException("Currency is required.");
            Price = newPrice;
        }

        public void IncreaseStock(int qty)
        {
            if (qty <= 0) throw new DomainException("Quantity must be positive.");
            Stock += qty;
        }

        public void ReduceStock(int qty)
        {
            if (qty <= 0) throw new DomainException("Quantity must be positive.");
            if (Stock < qty) throw new DomainException("Not enough stock.");
            Stock -= qty;

            if (Stock == 0)
                AddDomainEvent(new ProductOutOfStock(Id.Value));
        }

        // --- Validation ---
        public bool IsAvailable(int qty) => Stock >= qty && IsActive;

        // --- Workflow ---
        public void Deactivate()
        {
            if (Stock > 0)
                throw new DomainException("Cannot deactivate a product with stock.");
            IsActive = false;
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
