using Finance.Application.Common;
using Finance.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.AddPayment
{
    public record AddPaymentCommand(
     Guid InvoiceId,
     decimal Amount,
     PaymentMethod Method,
     string? Note = null
 ) : IRequest<Result<bool>>;
}
