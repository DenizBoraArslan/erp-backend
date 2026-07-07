using System.Collections.Generic;

namespace Finance.Application.DTOs
{
    // One row per calendar month (format "yyyy-MM"), for the monthly chart.
    public record ProfitLossMonthDto(
        string Month,
        decimal Revenue,
        decimal Cost,
        decimal Profit
    );

    // One row per calendar year, for the yearly summary.
    public record ProfitLossYearDto(
        int Year,
        decimal Revenue,
        decimal Cost,
        decimal Profit
    );

    public record ProfitLossReportDto(
        decimal TotalRevenue,
        decimal TotalCost,
        decimal TotalProfit,
        List<ProfitLossMonthDto> Monthly,
        List<ProfitLossYearDto> Yearly
    );
}
