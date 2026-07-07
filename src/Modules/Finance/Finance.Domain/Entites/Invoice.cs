using Finance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    public class Invoice
    {
        public Guid Id { get; private set; }
        public string InvoiceNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; } = string.Empty;
        public Guid? OrderId { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal TaxTotal { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public DateTime IssuedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private List<InvoiceItem> _items = new();
        public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

        private List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        private Invoice() { }

        public static Invoice Create(Guid customerId, string customerName, DateTime? dueDate = null, Guid? orderId = null)
        {
            return new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                CustomerId = customerId,
                CustomerName = customerName,
                OrderId = orderId,
                Status = InvoiceStatus.Draft,
                TotalAmount = 0,
                PaidAmount = 0,
                IssuedAt = DateTime.UtcNow,
                DueDate = dueDate
            };
        }

        public void AddItem(string description, int quantity, decimal unitPrice, decimal taxRate = 0)
        {
            var item = InvoiceItem.Create(Id, description, quantity, unitPrice, taxRate);
            _items.Add(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(DateTime? dueDate, IEnumerable<(string Description, int Quantity, decimal UnitPrice, decimal TaxRate)> items)
        {
            if (Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be updated.");

            _items.Clear();
            foreach (var item in items)
                _items.Add(InvoiceItem.Create(Id, item.Description, item.Quantity, item.UnitPrice, item.TaxRate));

            DueDate = dueDate;
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Issue()
        {
            if (Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be issued.");
            if (!_items.Any())
                throw new InvalidOperationException("Invoice must have at least one item.");
            Status = InvoiceStatus.Issued;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddPayment(decimal amount, PaymentMethod method, string? note = null)
        {
            if (Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot add payment to a cancelled invoice.");
            if (Status == InvoiceStatus.Paid)
                throw new InvalidOperationException("Invoice is already fully paid.");
            if (amount > RemainingAmount)
                throw new InvalidOperationException("Payment amount exceeds remaining balance.");

            var payment = Payment.Create(Id, amount, method, note);
            _payments.Add(payment);
            PaidAmount += amount;

            Status = PaidAmount >= TotalAmount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == InvoiceStatus.Paid)
                throw new InvalidOperationException("Paid invoices cannot be cancelled.");
            Status = InvoiceStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateTotal()
        {
            SubTotal = _items.Sum(i => i.TotalPrice);
            TaxTotal = _items.Sum(i => i.TaxAmount);
            TotalAmount = SubTotal + TaxTotal;
        }
    }
}
