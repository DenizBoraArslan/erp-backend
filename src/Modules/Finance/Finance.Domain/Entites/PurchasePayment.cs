using Finance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    // Mirrors Payment, but recorded against a PurchaseInvoice (money going OUT
    // to a supplier) instead of an Invoice (money coming IN from a customer).
    public class PurchasePayment
    {
        public Guid Id { get; private set; }
        public Guid PurchaseInvoiceId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; }
        public string? Note { get; private set; }
        public DateTime PaidAt { get; private set; }

        private PurchasePayment() { }

        public static PurchasePayment Create(Guid purchaseInvoiceId, decimal amount, PaymentMethod method, string? note = null)
        {
            return new PurchasePayment
            {
                Id = Guid.NewGuid(),
                PurchaseInvoiceId = purchaseInvoiceId,
                Amount = amount,
                Method = method,
                Note = note,
                PaidAt = DateTime.UtcNow
            };
        }
    }
}
