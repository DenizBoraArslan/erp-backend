using HR.Application.Features.Employees.CreateEmployee;
using HR.Application.Features.Employees.DeactivateEmployee;
using HR.Application.Features.Employees.GetEmployees;
using HR.Application.Features.Employees.UpdateEmployee;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetEmployeesQuery(), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin,HRManager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        [Authorize(Roles = "Admin,HRManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken ct)
        {
            var command = new UpdateEmployeeCommand(id, request.FirstName, request.LastName, request.Position, request.Salary, request.Phone);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { id = result.Data });
        }

        [Authorize(Roles = "Admin,HRManager")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeactivateEmployeeCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }

    public record UpdateEmployeeRequest(
        string FirstName,
        string LastName,
        string Position,
        decimal Salary,
        string? Phone = null
    );
}
