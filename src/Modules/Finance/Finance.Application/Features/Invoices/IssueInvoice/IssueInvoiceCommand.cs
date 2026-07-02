using Finance.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.IssueInvoice
{
    public record IssueInvoiceCommand(Guid InvoiceId) : IRequest<Result<bool>>;
}
