using Finance.Application.Features.Statements.GetCustomerStatement;
using Finance.Application.Features.Statements.GetSupplierStatement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/statements")]
    public class StatementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerStatement(Guid customerId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCustomerStatementQuery(customerId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<IActionResult> GetSupplierStatement(Guid supplierId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSupplierStatementQuery(supplierId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }
    }
}
