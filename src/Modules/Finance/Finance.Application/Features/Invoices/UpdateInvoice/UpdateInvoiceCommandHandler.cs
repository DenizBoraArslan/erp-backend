using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Result<bool>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result<bool>> Handle(UpdateInvoiceCommand request, CancellationToken ct)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Fatura bulunamadı.");

            if (request.Items == null || request.Items.Count == 0)
                return Result<bool>.Failure("Fatura en az bir kalem içermelidir.");

            try
            {
                invoice.UpdateDetails(
                    request.DueDate,
                    request.Items.Select(i => (i.Description, i.Quantity, i.UnitPrice, i.TaxRate))
                );
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
