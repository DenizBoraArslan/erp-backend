using Finance.Application.Common;
using Finance.Application.DTOs;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.GetInvoices
{
    public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, Result<IEnumerable<InvoiceDto>>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result<IEnumerable<InvoiceDto>>> Handle(GetInvoicesQuery request, CancellationToken ct)
        {
            var invoices = request.CustomerId.HasValue
                ? await _invoiceRepository.GetByCustomerIdAsync(request.CustomerId.Value, ct)
                : await _invoiceRepository.GetAllAsync(ct);

            var dtos = invoices.Select(i => new InvoiceDto(
                i.Id,
                i.InvoiceNumber,
                i.CustomerId,
                i.CustomerName,
                i.OrderId,
                i.Status.ToString(),
                i.SubTotal,
                i.TaxTotal,
                i.TotalAmount,
                i.PaidAmount,
                i.RemainingAmount,
                i.IssuedAt,
                i.DueDate,
                i.Items.Select(x => new InvoiceItemDto(x.Id, x.Description, x.Quantity, x.UnitPrice, x.TotalPrice, x.TaxRate, x.TaxAmount, x.GrandTotal)).ToList(),
                i.Payments.Select(p => new PaymentDto(p.Id, p.Amount, p.Method.ToString(), p.Note, p.PaidAt)).ToList()
            ));

            return Result<IEnumerable<InvoiceDto>>.Success(dtos);
        }
    }
}
