using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice
{
    public class UpdatePurchaseInvoiceCommandHandler : IRequestHandler<UpdatePurchaseInvoiceCommand, Result<bool>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;

        public UpdatePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
        }

        public async Task<Result<bool>> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken ct)
        {
            var invoice = await _purchaseInvoiceRepository.GetByIdAsync(request.PurchaseInvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Alım faturası bulunamadı.");

            if (request.Items == null || request.Items.Count == 0)
                return Result<bool>.Failure("Fatura en az bir kalem içermelidir.");

            try
            {
                invoice.UpdateDetails(
                    request.DueDate,
                    request.SupplierInvoiceNumber,
                    request.Items.Select(i => (i.Description, i.Quantity, i.UnitPrice, i.TaxRate, i.ProductId))
                );
            }
            catch (InvalidOperationException ex)
            {
                return Result<bool>.Failure(ex.Message);
            }

            await _purchaseInvoiceRepository.UpdateAsync(invoice, ct);
            return Result<bool>.Success(true);
        }
    }
}
