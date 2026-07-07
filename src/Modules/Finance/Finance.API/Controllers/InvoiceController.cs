using Finance.Application.Features.Invoices.AddPayment;
using Finance.Application.Features.Invoices.CreateInvoice;
using Finance.Application.Features.Invoices.GetInvoices;
using Finance.Application.Features.Invoices.IssueInvoice;
using Finance.Application.Features.Invoices.MarkAsPaid;
using Finance.Application.Features.Invoices.UpdateInvoice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPatch("{id}/mark-as-paid")]
        public async Task<IActionResult> MarkAsPaid(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkAsPaidCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? customerId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetInvoicesQuery(customerId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        // Issuing a Draft invoice (locking it in) is a manager action.
        [Authorize(Roles = "Admin,FinanceManager")]
        [HttpPatch("{id}/issue")]
        public async Task<IActionResult> Issue(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new IssueInvoiceCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPost("{id}/payments")]
        public async Task<IActionResult> AddPayment(Guid id, [FromBody] AddPaymentCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command with { InvoiceId = id }, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceRequest request, CancellationToken ct)
        {
            var command = new UpdateInvoiceCommand(id, request.Items, request.DueDate);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }

    public record UpdateInvoiceRequest(
        List<UpdateInvoiceItemRequest> Items,
        DateTime? DueDate = null
    );
}
