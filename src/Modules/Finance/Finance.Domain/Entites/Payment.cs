using Finance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid InvoiceId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; }
        public string? Note { get; private set; }
        public DateTime PaidAt { get; private set; }

        private Payment() { }

        public static Payment Create(Guid invoiceId, decimal amount, PaymentMethod method, string? note = null)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoiceId,
                Amount = amount,
                Method = method,
                Note = note,
                PaidAt = DateTime.UtcNow
            };
        }
    }
}
