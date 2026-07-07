using Finance.Application.Common;
using Finance.Application.DTOs;
using MediatR;

namespace Finance.Application.Features.Reports.GetProfitLossReport
{
    public record GetProfitLossReportQuery() : IRequest<Result<ProfitLossReportDto>>;
}
