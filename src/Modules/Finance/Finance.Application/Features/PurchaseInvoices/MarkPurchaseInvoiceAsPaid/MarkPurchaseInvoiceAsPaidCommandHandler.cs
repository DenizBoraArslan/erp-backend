using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.PurchaseInvoices.MarkPurchaseInvoiceAsPaid
{
    public class MarkPurchaseInvoiceAsPaidCommandHandler : IRequestHandler<MarkPurchaseInvoiceAsPaidCommand, Result<bool>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
        private readonly IPurchasePaymentRepository _purchasePaymentRepository;

        public MarkPurchaseInvoiceAsPaidCommandHandler(
            IPurchaseInvoiceRepository purchaseInvoiceRepository,
            IPurchasePaymentRepository purchasePaymentRepository)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _purchasePaymentRepository = purchasePaymentRepository;
        }

        public async Task<Result<bool>> Handle(MarkPurchaseInvoiceAsPaidCommand request, CancellationToken ct)
        {
            var invoice = await _purchaseInvoiceRepository.GetByIdAsync(request.PurchaseInvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Alım faturası bulunamadı.");

            await _purchaseInvoiceRepository.MarkAsPaidAsync(request.PurchaseInvoiceId, ct);

            var payment = PurchasePayment.Create(
                request.PurchaseInvoiceId,
                invoice.TotalAmount,
                Finance.Domain.Enums.PaymentMethod.Cash,
                "Tamamı ödendi"
            );

            await _purchasePaymentRepository.AddAsync(payment, ct);

            return Result<bool>.Success(true);
        }
    }
}
