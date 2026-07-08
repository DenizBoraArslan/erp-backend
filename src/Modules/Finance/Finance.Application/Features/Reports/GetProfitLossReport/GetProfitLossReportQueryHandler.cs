using Finance.Application.Common;
using Finance.Application.DTOs;
using Finance.Domain.Enums;
using Finance.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Reports.GetProfitLossReport
{
    public class GetProfitLossReportQueryHandler : IRequestHandler<GetProfitLossReportQuery, Result<ProfitLossReportDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;

        public GetProfitLossReportQueryHandler(
            IInvoiceRepository invoiceRepository,
            IPurchaseInvoiceRepository purchaseInvoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
        }

        public async Task<Result<ProfitLossReportDto>> Handle(GetProfitLossReportQuery request, CancellationToken ct)
        {
            var invoices = await _invoiceRepository.GetAllAsync(ct);
            var purchaseInvoices = await _purchaseInvoiceRepository.GetAllAsync(ct);

            var revenueInvoices = invoices
                .Where(i => i.Status != Finance.Domain.Enums.InvoiceStatus.Draft
                         && i.Status != Finance.Domain.Enums.InvoiceStatus.Cancelled)
                .ToList();

            var costInvoices = purchaseInvoices
                .Where(i => i.Status != PurchaseInvoiceStatus.Draft
                         && i.Status != PurchaseInvoiceStatus.Cancelled)
                .ToList();

            // Not: KDV, işletmenin kendi kâr/zararı değil devlete aktarılan bir
            // vergi olduğu için SubTotal (KDV hariç tutar) baz alınıyor.
            var revenueByMonth = revenueInvoices
                .GroupBy(i => $"{i.IssuedAt.Year:D4}-{i.IssuedAt.Month:D2}")
                .ToDictionary(g => g.Key, g => g.Sum(i => i.SubTotal));

            var costByMonth = costInvoices
                .GroupBy(i => $"{i.IssuedAt.Year:D4}-{i.IssuedAt.Month:D2}")
                .ToDictionary(g => g.Key, g => g.Sum(i => i.SubTotal));

            var allMonths = revenueByMonth.Keys.Union(costByMonth.Keys).OrderBy(m => m).ToList();

            var monthly = allMonths.Select(month =>
            {
                var revenue = revenueByMonth.GetValueOrDefault(month, 0m);
                var cost = costByMonth.GetValueOrDefault(month, 0m);
                return new ProfitLossMonthDto(month, revenue, cost, revenue - cost);
            }).ToList();

            var revenueByYear = revenueInvoices
                .GroupBy(i => i.IssuedAt.Year)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.SubTotal));

            var costByYear = costInvoices
                .GroupBy(i => i.IssuedAt.Year)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.SubTotal));

            var allYears = revenueByYear.Keys.Union(costByYear.Keys).OrderBy(y => y).ToList();

            var yearly = allYears.Select(year =>
            {
                var revenue = revenueByYear.GetValueOrDefault(year, 0m);
                var cost = costByYear.GetValueOrDefault(year, 0m);
                return new ProfitLossYearDto(year, revenue, cost, revenue - cost);
            }).ToList();

            var totalRevenue = revenueInvoices.Sum(i => i.SubTotal);
            var totalCost = costInvoices.Sum(i => i.SubTotal);

            var report = new ProfitLossReportDto(totalRevenue, totalCost, totalRevenue - totalCost, monthly, yearly);

            return Result<ProfitLossReportDto>.Success(report);
        }
    }
}
