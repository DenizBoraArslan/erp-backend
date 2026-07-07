using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.PurchaseInvoices.CreatePurchaseInvoice
{
    public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, Result<Guid>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;

        public CreatePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
        }

        public async Task<Result<Guid>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken ct)
        {
            if (request.Items == null || request.Items.Count == 0)
                return Result<Guid>.Failure("Fatura en az bir kalem içermelidir.");

            var invoice = PurchaseInvoice.Create(request.SupplierId, request.SupplierName, request.DueDate, request.SupplierInvoiceNumber);

            foreach (var item in request.Items)
                invoice.AddItem(item.Description, item.Quantity, item.UnitPrice, item.TaxRate, item.ProductId);

            await _purchaseInvoiceRepository.AddAsync(invoice, ct);

            return Result<Guid>.Success(invoice.Id);
        }
    }
}
