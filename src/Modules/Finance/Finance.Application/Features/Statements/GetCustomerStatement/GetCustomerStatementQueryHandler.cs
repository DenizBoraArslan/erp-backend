using Finance.Application.Common;
using Finance.Application.DTOs;
using Finance.Domain.Enums;
using Finance.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Statements.GetCustomerStatement
{
    public class GetCustomerStatementQueryHandler : IRequestHandler<GetCustomerStatementQuery, Result<StatementDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetCustomerStatementQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result<StatementDto>> Handle(GetCustomerStatementQuery request, CancellationToken ct)
        {
            var invoices = await _invoiceRepository.GetByCustomerIdAsync(request.CustomerId, ct);

            var relevantInvoices = invoices
                .Where(i => i.Status != InvoiceStatus.Draft && i.Status != InvoiceStatus.Cancelled)
                .ToList();

            var rawEntries = new List<(System.DateTime Date, string Type, string Description, decimal Debit, decimal Credit)>();

            foreach (var invoice in relevantInvoices)
            {
                rawEntries.Add((invoice.IssuedAt, "Invoice", $"Fatura {invoice.InvoiceNumber}", invoice.TotalAmount, 0m));

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
