using Finance.Application.Common;
using Finance.Application.DTOs;
using Finance.Domain.Enums;
using Finance.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Statements.GetSupplierStatement
{
    public class GetSupplierStatementQueryHandler : IRequestHandler<GetSupplierStatementQuery, Result<StatementDto>>
    {
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;

        public GetSupplierStatementQueryHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository)
        {
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
        }

        public async Task<Result<StatementDto>> Handle(GetSupplierStatementQuery request, CancellationToken ct)
        {
            var invoices = await _purchaseInvoiceRepository.GetBySupplierIdAsync(request.SupplierId, ct);

            var relevantInvoices = invoices
                .Where(i => i.Status != PurchaseInvoiceStatus.Draft && i.Status != PurchaseInvoiceStatus.Cancelled)
                .ToList();

            var rawEntries = new List<(System.DateTime Date, string Type, string Description, decimal Debit, decimal Credit)>();

            foreach (var invoice in relevantInvoices)
            {
                rawEntries.Add((invoice.IssuedAt, "Invoice", $"Alım Faturası {invoice.InvoiceNumber}", invoice.TotalAmount, 0m));

                foreach (var payment in invoice.Payments)
                    rawEntries.Add((payment.PaidAt, "Payment", $"Ödeme — {invoice.InvoiceNumber}", 0m, payment.Amount));
            }

            var ordered = rawEntries.OrderBy(e => e.Date).ToList();

            decimal runningBalance = 0m;
            var entries = new List<StatementEntryDto>();
            foreach (var e in ordered)
            {
                runningBalance += e.Debit - e.Credit;
                entries.Add(new StatementEntryDto(e.Date, e.Type, e.Description, e.Debit, e.Credit, runningBalance));
            }

            var totalDebit = ordered.Sum(e => e.Debit);
            var totalCredit = ordered.Sum(e => e.Credit);

            var dto = new StatementDto(entries, totalDebit, totalCredit, totalDebit - totalCredit);
            return Result<StatementDto>.Success(dto);
        }
    }
}
