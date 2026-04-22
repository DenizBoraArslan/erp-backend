using Common.Models;

namespace Purchase.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public SupplierType Type { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class PurchaseOrderLine : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public enum SupplierType
{
    Distributor = 0,
    Manufacturer = 1,
    Wholesaler = 2
}

public enum PurchaseOrderStatus
{
    Draft = 0,
    Pending = 1,
    Confirmed = 2,
    Received = 3,
    Invoiced = 4,
    Cancelled = 5
}
