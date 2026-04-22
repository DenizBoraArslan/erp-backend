using Common.Models;

namespace Sales.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public CustomerType Type { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SalesOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class OrderLine : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public enum CustomerType
{
    Retail = 0,
    Wholesale = 1,
    Corporate = 2
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
