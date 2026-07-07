using Finance.Application.Features.Suppliers.ActivateSupplier;
using Finance.Application.Features.Suppliers.CreateSupplier;
using Finance.Application.Features.Suppliers.DeactivateSupplier;
using Finance.Application.Features.Suppliers.GetSuppliers;
using Finance.Application.Features.Suppliers.UpdateSupplier;
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
    [Route("api/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupplierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSuppliersQuery(), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequest request, CancellationToken ct)
        {
            var command = new UpdateSupplierCommand(id, request.Name, request.ContactName, request.Phone, request.Address, request.TaxNumber);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        // Deactivating/reactivating a supplier is a manager-level action.
        [Authorize(Roles = "Admin,FinanceManager")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeactivateSupplierCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,FinanceManager")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new ActivateSupplierCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }

    public record UpdateSupplierRequest(
        string Name,
        string? ContactName = null,
        string? Phone = null,
        string? Address = null,
        string? TaxNumber = null
    );
}
