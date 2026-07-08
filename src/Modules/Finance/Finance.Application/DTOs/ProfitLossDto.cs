using System.Collections.Generic;

namespace Finance.Application.DTOs
{
   
    public record ProfitLossMonthDto(
        string Month,
        decimal Revenue,
        decimal Cost,
        decimal Profit
    );

  
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
