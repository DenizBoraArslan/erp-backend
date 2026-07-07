using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    // Tedarikçi — the purchasing-side counterpart to Sales' Customer. Represents
    // whoever we buy goods/services from; PurchaseInvoice records the cost side
    // of the P&L, mirroring how Customer + Invoice record the revenue side.
    public class Supplier
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? ContactName { get; private set; }
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public string? Address { get; private set; }
        public string? TaxNumber { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Supplier() { }

        public static Supplier Create(
            string name,
            string? contactName = null,
            string? email = null,
            string? phone = null,
            string? address = null,
            string? taxNumber = null)
        {
            return new Supplier
            {
                Id = Guid.NewGuid(),
                Name = name,
                ContactName = contactName,
                Email = string.IsNullOrWhiteSpace(email) ? null : email.ToLowerInvariant(),
                Phone = phone,
                Address = address,
                TaxNumber = taxNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string name, string? contactName, string? phone, string? address, string? taxNumber)
        {
            Name = name;
            ContactName = contactName;
            Phone = phone;
            Address = address;
            TaxNumber = taxNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
