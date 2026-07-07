namespace erp.Shared.Events;

// Published when an Alım Faturası (Purchase Invoice) is confirmed. Inventory
// consumes this to increment stock for each line item that was linked to a
// tracked product (ProductId is optional on a purchase invoice item — not
// every cost line, e.g. freight or services, corresponds to a stock item),
// logging a StockMovement (Type=In).
public record PurchaseInvoiceConfirmedEvent
{
    public Guid PurchaseInvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public List<PurchaseInvoiceConfirmedEventItem> Items { get; init; } = new();
}

public record PurchaseInvoiceConfirmedEventItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
