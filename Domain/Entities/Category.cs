using Domain.Exceptions;
using System;

namespace Domain.Entities
{
    public class Category
    {
        public CategoryId Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        private Category() { }

        public Category(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");

            Id = CategoryId.NewId();
            Name = name;
            Description = description;
        }
    }

    public readonly record struct CategoryId(Guid Value)
    {
        public static CategoryId NewId() => new CategoryId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}