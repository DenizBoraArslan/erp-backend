using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entites
{
    public class PurchaseInvoiceItem
    {
        public Guid Id { get; private set; }
        public Guid PurchaseInvoiceId { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        // KDV oranı (%), örn. 20 = %20. Varsayılan 0 (vergisiz/muaf kalemler için).
        public decimal TaxRate { get; private set; }
        // Opsiyonel: bu kalem Inventory modülündeki bir ürüne karşılık
        // geliyorsa ProductId set edilir; fatura onaylandığında bu ürünün
        // stoğu otomatik artırılır ve StockMovement kaydı oluşturulur.
        // Nakliye/hizmet gibi stok dışı kalemler için null bırakılabilir.
        public Guid? ProductId { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public decimal TaxAmount => TotalPrice * TaxRate / 100m;
        public decimal GrandTotal => TotalPrice + TaxAmount;

        private PurchaseInvoiceItem() { }

        public static PurchaseInvoiceItem Create(Guid purchaseInvoiceId, string description, int quantity, decimal unitPrice, decimal taxRate = 0, Guid? productId = null)
        {
            return new PurchaseInvoiceItem
            {
                Id = Guid.NewGuid(),
                PurchaseInvoiceId = purchaseInvoiceId,
                Description = description,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TaxRate = taxRate,
                ProductId = productId
            };
        }
    }
}
