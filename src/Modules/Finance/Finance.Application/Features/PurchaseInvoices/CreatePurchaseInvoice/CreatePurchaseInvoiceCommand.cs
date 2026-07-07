using Finance.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace Finance.Application.Features.PurchaseInvoices.CreatePurchaseInvoice
{
    public record PurchaseInvoiceItemRequest(
        string Description,
        int Quantity,
        decimal UnitPrice,
        decimal TaxRate = 0,
        Guid? ProductId = null
    );

    public record CreatePurchaseInvoiceCommand(
        Guid SupplierId,
        string SupplierName,
        DateTime DueDate,
        List<PurchaseInvoiceItemRequest> Items,
        string? SupplierInvoiceNumber = null
    ) : IRequest<Result<Guid>>;
}
