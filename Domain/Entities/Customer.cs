using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer
    {
        public CustomerId Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
    }
    public readonly record struct CustomerId(Guid Value)
    {
        public static CustomerId NewId() => new CustomerId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
