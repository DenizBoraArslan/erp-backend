using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Features.Customers.CreateCustomer;
using Sales.Application.Features.Customers.DeactivateCustomer;
using Sales.Application.Features.Customers.GetCustomers;
using Sales.Application.Features.Customers.UpdateCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCustomersQuery(), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin,SalesManager,SalesRep")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        [Authorize(Roles = "Admin,SalesManager,SalesRep")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken ct)
        {
            var command = new UpdateCustomerCommand(id, request.FirstName, request.LastName, request.Phone, request.Address);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { id = result.Data });
        }

        // Deleting/deactivating a customer is a manager-level action.
        [Authorize(Roles = "Admin,SalesManager")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeactivateCustomerCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }

    public record UpdateCustomerRequest(
        string FirstName,
        string LastName,
        string? Phone = null,
        string? Address = null
    );
}
