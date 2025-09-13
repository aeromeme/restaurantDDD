using System;

namespace Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }

        public string? ParamName { get; }

        public DomainException(string message, string? paramName = null)
            : base(message)
        {
            ParamName = paramName;
        }
    }
}