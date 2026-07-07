namespace erp.Shared.Events;

// Published when a Sales Order transitions Pending -> Confirmed. Inventory
// consumes this to decrement stock for each ordered product and log a
// StockMovement (Type=Out), giving a traceable link between a sale and the
// resulting stock change.
public record OrderConfirmedEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public List<OrderConfirmedEventItem> Items { get; init; } = new();
}

public record OrderConfirmedEventItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
