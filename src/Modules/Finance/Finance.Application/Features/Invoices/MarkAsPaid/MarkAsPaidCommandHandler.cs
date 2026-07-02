using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;

namespace Finance.Application.Features.Invoices.MarkAsPaid
{
    public class MarkAsPaidCommandHandler : IRequestHandler<MarkAsPaidCommand, Result<bool>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;

        public MarkAsPaidCommandHandler(IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<bool>> Handle(MarkAsPaidCommand request, CancellationToken ct)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Invoice not found.");

            await _invoiceRepository.MarkAsPaidAsync(request.InvoiceId, ct);

            var payment = Payment.Create(
                request.InvoiceId,
                invoice.TotalAmount,
                Finance.Domain.Enums.PaymentMethod.Cash,
                "Paid in full"
            );

            Console.WriteLine($"Creating payment for invoice {request.InvoiceId}, amount: {invoice.TotalAmount}");

            await _paymentRepository.AddAsync(payment, ct);

            Console.WriteLine($"Payment created successfully");

            return Result<bool>.Success(true);
        }
    }
}
