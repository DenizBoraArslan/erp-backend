using erp.Shared.Events;
using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MassTransit;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.PurchaseInvoices.ConfirmPurchaseInvoice
{
    public class ConfirmPurchaseInvoiceCommandHandler : IRequestHandler<ConfirmPurchaseInvoiceCommand, Result<bool>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ConfirmPurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, IPublishEndpoint publishEndpoint)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<bool>> Handle(ConfirmPurchaseInvoiceCommand request, CancellationToken ct)
        {
            var invoice = await _purchaseInvoiceRepository.GetByIdAsync(request.PurchaseInvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Alım faturası bulunamadı.");

            try
            {
                invoice.Confirm();
            }
            catch (InvalidOperationException ex)
            {
                return Result<bool>.Failure(ex.Message);
            }

            await _purchaseInvoiceRepository.UpdateAsync(invoice, ct);

            var stockItems = invoice.Items.Where(i => i.ProductId.HasValue).ToList();
            if (stockItems.Any())
            {
                await _publishEndpoint.Publish(new PurchaseInvoiceConfirmedEvent
                {
                    PurchaseInvoiceId = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    Items = stockItems.Select(i => new PurchaseInvoiceConfirmedEventItem
                    {
                        ProductId = i.ProductId!.Value,
                        Quantity = i.Quantity
                    }).ToList()
                }, ct);
            }

            return Result<bool>.Success(true);
        }
    }
}
