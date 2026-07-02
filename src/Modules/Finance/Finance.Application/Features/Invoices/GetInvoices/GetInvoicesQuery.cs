using Finance.Application.Common;
using Finance.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.GetInvoices
{
    public record GetInvoicesQuery(Guid? CustomerId = null) : IRequest<Result<IEnumerable<InvoiceDto>>>;
}
