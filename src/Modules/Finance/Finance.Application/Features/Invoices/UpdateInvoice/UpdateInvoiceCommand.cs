using Finance.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace Finance.Application.Features.Invoices.UpdateInvoice
{
    public record UpdateInvoiceItemRequest(
        string Description,
        int Quantity,
        decimal UnitPrice,
        decimal TaxRate = 0
    );

    public record UpdateInvoiceCommand(
        Guid InvoiceId,
        List<UpdateInvoiceItemRequest> Items,
        DateTime? DueDate = null
    ) : IRequest<Result<bool>>;
}
