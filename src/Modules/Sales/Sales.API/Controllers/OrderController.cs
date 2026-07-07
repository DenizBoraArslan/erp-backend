using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Features.Orders.CreateOrder;
using Sales.Application.Features.Orders.GetOrders;
using Sales.Application.Features.Orders.UpdateOrder;
using Sales.Application.Features.Orders.UpdateOrderStatus;

namespace Sales.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? customerId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetOrdersQuery(customerId), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin,SalesManager,SalesRep")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return CreatedAtAction(nameof(GetAll), new { id = result.Data });
        }

        [Authorize(Roles = "Admin,SalesManager,SalesRep")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string action, CancellationToken ct)
        {
            var result = await _mediator.Send(new UpdateOrderStatusCommand(id, action), ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }

        [Authorize(Roles = "Admin,SalesManager,SalesRep")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken ct)
        {
            var command = new UpdateOrderCommand(id, request.Items, request.Note);
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok();
        }
    }

    public record UpdateOrderRequest(
        List<UpdateOrderItemRequest> Items,
        string? Note = null
    );
}
