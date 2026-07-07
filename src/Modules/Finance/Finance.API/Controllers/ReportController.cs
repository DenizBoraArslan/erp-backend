using Finance.Application.Features.Reports.GetProfitLossReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Profit margins are more sensitive than a plain invoice list, so
        // this is locked to Finance roles + Admin only (unlike GetAll on
        // Invoice/PurchaseInvoice, which any authenticated role can call).
        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpGet("profit-loss")]
        public async Task<IActionResult> GetProfitLoss(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetProfitLossReportQuery(), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }
    }
}
