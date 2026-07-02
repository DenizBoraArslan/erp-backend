using HR.Application.Features.LeaveRequests.CreateLeaveRequest;
using HR.Application.Features.LeaveRequests.GetLeaveRequests;
using HR.Application.Features.LeaveRequests.ReviewLeaveRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.API.Controllers
{
    [ApiController]
    [Route("api/leave-requests")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetLeaveRequestsQuery(employeeId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        [HttpPatch("{id}/review")]
        public async Task<IActionResult> Review(Guid id, [FromBody] ReviewLeaveRequestCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command with { LeaveRequestId = id }, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }
}
