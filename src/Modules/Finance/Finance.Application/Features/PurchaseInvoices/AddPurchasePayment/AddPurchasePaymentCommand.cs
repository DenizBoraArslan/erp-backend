using Finance.Application.Common;
using Finance.Domain.Enums;
using MediatR;
using System;

namespace Finance.Application.Features.PurchaseInvoices.AddPurchasePayment
{
    public record AddPurchasePaymentCommand(
        Guid PurchaseInvoiceId,
        decimal Amount,
        PaymentMethod Method,
        string? Note = null
    ) : IRequest<Result<bool>>;
}
