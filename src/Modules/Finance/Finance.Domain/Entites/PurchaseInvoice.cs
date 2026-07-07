using Finance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    // Alım Faturası — the cost-side counterpart to Invoice (which only ever
    // represents outgoing sales invoices to customers). This is what a
    // supplier bills US for; combined with Invoice.TotalAmount (revenue) it
    // lets us compute Kâr/Zarar (profit/loss) = revenue - cost.
    public class PurchaseInvoice
    {
        public Guid Id { get; private set; }
        public string InvoiceNumber { get; private set; } = string.Empty;
        public Guid SupplierId { get; private set; }
        public string SupplierName { get; private set; } = string.Empty;
        public string? SupplierInvoiceNumber { get; private set; }
        public PurchaseInvoiceStatus Status { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal TaxTotal { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public DateTime IssuedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private List<PurchaseInvoiceItem> _items = new();
        public IReadOnlyCollection<PurchaseInvoiceItem> Items => _items.AsReadOnly();

        private List<PurchasePayment> _payments = new();
        public IReadOnlyCollection<PurchasePayment> Payments => _payments.AsReadOnly();

        private PurchaseInvoice() { }

        public static PurchaseInvoice Create(
            Guid supplierId,
            string supplierName,
            DateTime? dueDate = null,
            string? supplierInvoiceNumber = null)
        {
            return new PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = $"PUR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                SupplierId = supplierId,
                SupplierName = supplierName,
                SupplierInvoiceNumber = supplierInvoiceNumber,
                Status = PurchaseInvoiceStatus.Draft,
                TotalAmount = 0,
                PaidAmount = 0,
                IssuedAt = DateTime.UtcNow,
                DueDate = dueDate
            };
        }

        public void AddItem(string description, int quantity, decimal unitPrice, decimal taxRate = 0, Guid? productId = null)
        {
            var item = PurchaseInvoiceItem.Create(Id, description, quantity, unitPrice, taxRate, productId);
            _items.Add(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(
            DateTime? dueDate,
            string? supplierInvoiceNumber,
            IEnumerable<(string Description, int Quantity, decimal UnitPrice, decimal TaxRate, Guid? ProductId)> items)
        {
            if (Status != PurchaseInvoiceStatus.Draft)
                throw new InvalidOperationException("Sadece taslak durumundaki alım faturaları güncellenebilir.");

            _items.Clear();
            foreach (var item in items)
                _items.Add(PurchaseInvoiceItem.Create(Id, item.Description, item.Quantity, item.UnitPrice, item.TaxRate, item.ProductId));

            DueDate = dueDate;
            SupplierInvoiceNumber = supplierInvoiceNumber;
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm()
        {
            if (Status != PurchaseInvoiceStatus.Draft)
                throw new InvalidOperationException("Sadece taslak durumundaki alım faturaları onaylanabilir.");
            if (!_items.Any())
                throw new InvalidOperationException("Faturada en az bir kalem olmalıdır.");
            Status = PurchaseInvoiceStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddPayment(decimal amount, PaymentMethod method, string? note = null)
        {
            if (Status == PurchaseInvoiceStatus.Cancelled)
                throw new InvalidOperationException("İptal edilmiş faturaya ödeme eklenemez.");
            if (Status == PurchaseInvoiceStatus.Paid)
                throw new InvalidOperationException("Fatura zaten tamamen ödenmiş.");
            if (amount > RemainingAmount)
                throw new InvalidOperationException("Ödeme tutarı kalan bakiyeyi aşamaz.");

            var payment = PurchasePayment.Create(Id, amount, method, note);
            _payments.Add(payment);
            PaidAmount += amount;

            Status = PaidAmount >= TotalAmount ? PurchaseInvoiceStatus.Paid : PurchaseInvoiceStatus.PartiallyPaid;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == PurchaseInvoiceStatus.Paid)
                throw new InvalidOperationException("Ödenmiş faturalar iptal edilemez.");
            Status = PurchaseInvoiceStatus.Cancelled;
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
