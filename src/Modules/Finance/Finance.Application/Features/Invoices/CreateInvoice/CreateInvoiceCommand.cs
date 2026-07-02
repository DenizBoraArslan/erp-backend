using Finance.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.CreateInvoice
{
    public record InvoiceItemRequest(
     string Description,
     int Quantity,
     decimal UnitPrice
 );

    public record CreateInvoiceCommand(
        Guid CustomerId,
        string CustomerName,
        DateTime DueDate,
        List<InvoiceItemRequest> Items,
        Guid? OrderId = null
    ) : IRequest<Result<Guid>>;
}
