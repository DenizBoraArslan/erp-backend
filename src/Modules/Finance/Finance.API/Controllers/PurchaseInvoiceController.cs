using Finance.Application.Features.PurchaseInvoices.AddPurchasePayment;
using Finance.Application.Features.PurchaseInvoices.ConfirmPurchaseInvoice;
using Finance.Application.Features.PurchaseInvoices.CreatePurchaseInvoice;
using Finance.Application.Features.PurchaseInvoices.GetPurchaseInvoices;
using Finance.Application.Features.PurchaseInvoices.MarkPurchaseInvoiceAsPaid;
using Finance.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice;
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
    [Route("api/purchase-invoices")]
    public class PurchaseInvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchaseInvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? supplierId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetPurchaseInvoicesQuery(supplierId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseInvoiceCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseInvoiceRequest request, CancellationToken ct)
        {
            var command = new UpdatePurchaseInvoiceCommand(id, request.Items, request.DueDate, request.SupplierInvoiceNumber);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,FinanceManager")]
        [HttpPatch("{id}/confirm")]
        public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new ConfirmPurchaseInvoiceCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPost("{id}/payments")]
        public async Task<IActionResult> AddPayment(Guid id, [FromBody] AddPurchasePaymentRequest request, CancellationToken ct)
        {
            var command = new AddPurchasePaymentCommand(id, request.Amount, request.Method, request.Note);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,FinanceManager,FinanceSpecialist")]
        [HttpPatch("{id}/mark-as-paid")]
        public async Task<IActionResult> MarkAsPaid(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkPurchaseInvoiceAsPaidCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }

    public record UpdatePurchaseInvoiceRequest(
        System.Collections.Generic.List<Finance.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice.UpdatePurchaseInvoiceItemRequest> Items,
        DateTime? DueDate = null,
        string? SupplierInvoiceNumber = null
    );

    public record AddPurchasePaymentRequest(
        decimal Amount,
        Finance.Domain.Enums.PaymentMethod Method,
        string? Note = null
    );
}
