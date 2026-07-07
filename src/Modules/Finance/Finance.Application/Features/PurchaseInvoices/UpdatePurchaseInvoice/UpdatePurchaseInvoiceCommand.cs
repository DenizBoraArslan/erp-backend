using Finance.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace Finance.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice
{
    public record UpdatePurchaseInvoiceItemRequest(
        string Description,
        int Quantity,
        decimal UnitPrice,
        decimal TaxRate = 0,
        Guid? ProductId = null
    );

    public record UpdatePurchaseInvoiceCommand(
        Guid PurchaseInvoiceId,
        List<UpdatePurchaseInvoiceItemRequest> Items,
        DateTime? DueDate = null,
        string? SupplierInvoiceNumber = null
    ) : IRequest<Result<bool>>;
}
