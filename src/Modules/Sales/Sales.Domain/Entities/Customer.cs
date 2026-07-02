using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public string? Address { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Customer() { }

        public static Customer Create(string firstName, string lastName, string email, string? phone = null, string? address = null)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email.ToLowerInvariant(),
                Phone = phone,
                Address = address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string firstName, string lastName, string? phone, string? address)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
        }

        public void Deactivate() => IsActive = false;
    }
}
