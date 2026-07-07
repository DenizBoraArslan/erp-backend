using System;
using System.Collections.Generic;

namespace Finance.Application.DTOs
{
    public record PurchaseInvoiceDto(
        Guid Id,
        string InvoiceNumber,
        Guid SupplierId,
        string SupplierName,
        string? SupplierInvoiceNumber,
        string Status,
        decimal SubTotal,
        decimal TaxTotal,
        decimal TotalAmount,
        decimal PaidAmount,
        decimal RemainingAmount,
        DateTime IssuedAt,
        DateTime? DueDate,
        List<PurchaseInvoiceItemDto> Items,
        List<PurchasePaymentDto> Payments
    );
}
