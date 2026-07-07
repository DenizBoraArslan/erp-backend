namespace erp.Shared.Contracts;

/// <summary>
/// Cross-module read port: lets the Sales module check a product's current
/// stock level (owned by Inventory) synchronously, so it can reject an order
/// up front instead of silently under-fulfilling it later. Stock is actually
/// decremented asynchronously via OrderConfirmedEvent -> Inventory's
/// OrderConfirmedConsumer, which can only skip an item on insufficient stock
/// (it has no way to hand a synchronous error back to the original caller).
/// Blocking here, before that event is ever published, is what lets Sales
/// return a real validation error to the user.
/// </summary>
public interface IProductStockProvider
{
    Task<ProductStockInfo?> GetStockInfoAsync(Guid productId, CancellationToken ct = default);
}

public record ProductStockInfo(int StockQuantity, int MinStockLevel, bool IsActive);
