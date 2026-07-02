using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.IssueInvoice
{
    public class IssueInvoiceCommandHandler : IRequestHandler<IssueInvoiceCommand, Result<bool>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public IssueInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result<bool>> Handle(IssueInvoiceCommand request, CancellationToken ct)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Invoice not found.");

            try
            {
                invoice.Issue();
            }
            catch (InvalidOperationException ex)
            {
                return Result<bool>.Failure(ex.Message);
            }

            await _invoiceRepository.UpdateAsync(invoice, ct);
            return Result<bool>.Success(true);
        }
    }
}
