using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Entities
{
    public class StockMovement
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public int Quantity { get; private set; }
        public MovementType Type { get; private set; }
        public string? Note { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private StockMovement() { }

        public static StockMovement Create(Guid productId, int quantity, MovementType type, string? note = null)
        {
            return new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = quantity,
                Type = type,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
