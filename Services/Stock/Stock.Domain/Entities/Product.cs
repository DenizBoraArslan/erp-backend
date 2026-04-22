using Common.Models;

namespace Stock.Domain.Entities;

public class Product : BaseEntity
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public MovementType Type { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public enum MovementType
{
    In = 0,
    Out = 1,
    Adjustment = 2,
    Damage = 3
}
