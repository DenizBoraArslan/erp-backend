using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.CreateInvoice
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<Guid>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken ct)
        {
            if (request.Items == null || request.Items.Count == 0)
                return Result<Guid>.Failure("Invoice must have at least one item.");

            var invoice = Invoice.Create(request.CustomerId, request.CustomerName, request.DueDate, request.OrderId);

            foreach (var item in request.Items)
                invoice.AddItem(item.Description, item.Quantity, item.UnitPrice, item.TaxRate);

            await _invoiceRepository.AddAsync(invoice, ct);

            return Result<Guid>.Success(invoice.Id);
        }
    }
}
