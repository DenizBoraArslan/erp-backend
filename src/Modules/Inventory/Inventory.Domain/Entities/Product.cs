using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string SKU { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public int MinStockLevel { get; private set; }
        public Guid CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Product() { }

        public static Product Create(string name, decimal price, Guid categoryId, string? description = null, int minStockLevel = 0)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                SKU = $"PRD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                Price = price,
                CategoryId = categoryId,
                Description = description,
                StockQuantity = 0,
                MinStockLevel = minStockLevel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string name, string? description, decimal price, int minStockLevel)
        {
            Name = name;
            Description = description;
            Price = price;
            MinStockLevel = minStockLevel;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");
            if (StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock.");
            StockQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsLowStock() => StockQuantity <= MinStockLevel;

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
