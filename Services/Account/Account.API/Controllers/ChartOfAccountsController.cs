using Microsoft.AspNetCore.Mvc;
using MediatR;
using Account.Application.Features.ChartOfAccounts.Commands;

namespace Account.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChartOfAccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChartOfAccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateChartOfAccount(CreateChartOfAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(CreateChartOfAccount), result);
    }
}
