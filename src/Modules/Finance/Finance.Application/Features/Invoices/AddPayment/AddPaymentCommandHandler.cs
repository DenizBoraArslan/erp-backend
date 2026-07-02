using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Features.Invoices.AddPayment
{
    public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, Result<bool>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;

        public AddPaymentCommandHandler(IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<bool>> Handle(AddPaymentCommand request, CancellationToken ct)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, ct);
            if (invoice is null)
                return Result<bool>.Failure("Invoice not found.");

            if (invoice.Status == Finance.Domain.Enums.InvoiceStatus.Cancelled)
                return Result<bool>.Failure("Cannot add payment to a cancelled invoice.");
            if (invoice.Status == Finance.Domain.Enums.InvoiceStatus.Paid)
                return Result<bool>.Failure("Invoice is already fully paid.");
            if (request.Amount > invoice.RemainingAmount)
                return Result<bool>.Failure("Payment amount exceeds remaining balance.");

            var payment = Payment.Create(request.InvoiceId, request.Amount, request.Method, request.Note);
            await _paymentRepository.AddAsync(payment, ct);

            await _invoiceRepository.UpdatePaidAmountAsync(request.InvoiceId, request.Amount, ct);

            return Result<bool>.Success(true);
        }
    }
}
