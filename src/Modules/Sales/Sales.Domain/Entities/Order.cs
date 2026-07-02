using Sales.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string OrderNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; private set; }
        public Customer? Customer { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string? Note { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        private Order() { }

        public static Order Create(Guid customerId, string? note = null)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                CustomerId = customerId,
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            var item = OrderItem.Create(Id, productId, productName, quantity, unitPrice);
            _orderItems.Add(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be confirmed.");
            Status = OrderStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Ship()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed orders can be shipped.");
            Status = OrderStatus.Shipped;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deliver()
        {
            if (Status != OrderStatus.Shipped)
                throw new InvalidOperationException("Only shipped orders can be delivered.");
            Status = OrderStatus.Delivered;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Delivered orders cannot be cancelled.");
            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateTotal()
        {
            TotalAmount = _orderItems.Sum(i => i.TotalPrice);
        }
    }
}
