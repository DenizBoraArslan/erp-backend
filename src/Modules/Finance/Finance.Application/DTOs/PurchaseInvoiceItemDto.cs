using System;

namespace Finance.Application.DTOs
{
    public record PurchaseInvoiceItemDto(
        Guid Id,
        string Description,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice,
        decimal TaxRate,
        decimal TaxAmount,
        decimal GrandTotal,
        Guid? ProductId
    );
}
