using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.PurchaseInvoices.AddPurchasePayment
{
    public class AddPurchasePaymentCommandHandler : IRequestHandler<AddPurchasePaymentCommand, Result<bool>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
        private readonly IPurchasePaymentRepository _purchasePaymentRepository;

        public AddPurchasePaymentCommandHandler(
            IPurchaseInvoiceRepository purchaseInvoiceRepository,
            IPurchasePaymentRepository purchasePaymentRepository)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _purchasePaymentRepository = purchasePaymentRepository;
        }

        public async Task<Result<bool>> Handle(AddPurchasePaymentCommand request, CancellationToken ct)
        {
            var invoice = await _purchaseInvoiceRepository.GetByIdAsync(request.PurchaseInvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Alım faturası bulunamadı.");

            if (invoice.Status == Finance.Domain.Enums.PurchaseInvoiceStatus.Cancelled)
                return Result<bool>.Failure("İptal edilmiş faturaya ödeme eklenemez.");
            if (invoice.Status == Finance.Domain.Enums.PurchaseInvoiceStatus.Paid)
                return Result<bool>.Failure("Fatura zaten tamamen ödenmiş.");
            if (request.Amount > invoice.RemainingAmount)
                return Result<bool>.Failure("Ödeme tutarı kalan bakiyeyi aşamaz.");

            var payment = PurchasePayment.Create(request.PurchaseInvoiceId, request.Amount, request.Method, request.Note);
            await _purchasePaymentRepository.AddAsync(payment, ct);

            await _purchaseInvoiceRepository.UpdatePaidAmountAsync(request.PurchaseInvoiceId, request.Amount, ct);

            return Result<bool>.Success(true);
        }
    }
}
