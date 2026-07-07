using Finance.Application.Common;
using MediatR;
using System;

namespace Finance.Application.Features.PurchaseInvoices.MarkPurchaseInvoiceAsPaid
{
    public record MarkPurchaseInvoiceAsPaidCommand(Guid PurchaseInvoiceId) : IRequest<Result<bool>>;
}
