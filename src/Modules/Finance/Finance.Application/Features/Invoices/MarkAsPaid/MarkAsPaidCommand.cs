using Finance.Application.Common;
using MediatR;

namespace Finance.Application.Features.Invoices.MarkAsPaid
{
    public record MarkAsPaidCommand(Guid InvoiceId) : IRequest<Result<bool>>;
}
