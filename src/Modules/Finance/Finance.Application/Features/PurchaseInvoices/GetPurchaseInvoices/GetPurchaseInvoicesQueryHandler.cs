using Finance.Application.Common;
using Finance.Application.DTOs;
using Finance.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.PurchaseInvoices.GetPurchaseInvoices
{
    public class GetPurchaseInvoicesQueryHandler : IRequestHandler<GetPurchaseInvoicesQuery, Result<IEnumerable<PurchaseInvoiceDto>>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;

        public GetPurchaseInvoicesQueryHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
        }

        public async Task<Result<IEnumerable<PurchaseInvoiceDto>>> Handle(GetPurchaseInvoicesQuery request, CancellationToken ct)
        {
            var invoices = request.SupplierId.HasValue
                ? await _purchaseInvoiceRepository.GetBySupplierIdAsync(request.SupplierId.Value, ct)
                : await _purchaseInvoiceRepository.GetAllAsync(ct);

            var dtos = invoices.Select(i => new PurchaseInvoiceDto(
                i.Id,
                i.InvoiceNumber,
                i.SupplierId,
                i.SupplierName,
                i.SupplierInvoiceNumber,
                i.Status.ToString(),
                i.SubTotal,
                i.TaxTotal,
                i.TotalAmount,
                i.PaidAmount,
                i.RemainingAmount,
                i.IssuedAt,
                i.DueDate,
                i.Items.Select(x => new PurchaseInvoiceItemDto(x.Id, x.Description, x.Quantity, x.UnitPrice, x.TotalPrice, x.TaxRate, x.TaxAmount, x.GrandTotal, x.ProductId)).ToList(),
                i.Payments.Select(p => new PurchasePaymentDto(p.Id, p.Amount, p.Method.ToString(), p.Note, p.PaidAt)).ToList()
            ));

            return Result<IEnumerable<PurchaseInvoiceDto>>.Success(dtos);
        }
    }
}
