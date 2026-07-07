using Finance.Application.Common;
using Finance.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace Finance.Application.Features.PurchaseInvoices.GetPurchaseInvoices
{
    public record GetPurchaseInvoicesQuery(Guid? SupplierId = null) : IRequest<Result<IEnumerable<PurchaseInvoiceDto>>>;
}
