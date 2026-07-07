using Inventory.Application.Features.StockMovements.GetStockMovements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/stock-movements")]
    public class StockMovementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockMovementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? productId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetStockMovementsQuery(productId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }
    }
}
