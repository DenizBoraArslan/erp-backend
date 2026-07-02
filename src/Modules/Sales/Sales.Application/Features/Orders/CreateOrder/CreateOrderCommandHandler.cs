namespace Sales.Application.Features.Orders.CreateOrder;

using erp.Shared.Events;
using MassTransit;
using MediatR;
using Sales.Application.Common;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
        if (customer is null)
            return Result<Guid>.Failure("Customer not found.");

        if (request.Items == null || request.Items.Count == 0)
            return Result<Guid>.Failure("Order must have at least one item.");

        var order = Order.Create(request.CustomerId, request.Note);

        foreach (var item in request.Items)
            order.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        await _orderRepository.AddAsync(order, ct);

        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = customer.Id,
            CustomerName = $"{customer.FirstName} {customer.LastName}",
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => new OrderCreatedEventItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }, ct);

        return Result<Guid>.Success(order.Id);
    }
}