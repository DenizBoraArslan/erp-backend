using Finance.Application.Common;
using MediatR;
using System;

namespace Finance.Application.Features.PurchaseInvoices.ConfirmPurchaseInvoice
{
    public record ConfirmPurchaseInvoiceCommand(Guid PurchaseInvoiceId) : IRequest<Result<bool>>;
}
