using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    public class InvoiceItem
    {
        public Guid Id { get; private set; }
        public Guid InvoiceId { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        // KDV oranı (%), örn. 20 = %20. Varsayılan 0 (vergisiz/muaf kalemler için).
        public decimal TaxRate { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public decimal TaxAmount => TotalPrice * TaxRate / 100m;
        public decimal GrandTotal => TotalPrice + TaxAmount;

        private InvoiceItem() { }

        public static InvoiceItem Create(Guid invoiceId, string description, int quantity, decimal unitPrice, decimal taxRate = 0)
        {
            return new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoiceId,
                Description = description,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TaxRate = taxRate
            };
        }
    }
}
