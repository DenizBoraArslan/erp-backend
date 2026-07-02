using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly InventoryDbContext _context;

        public StockMovementRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockMovement>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
            => await _context.StockMovements.Where(s => s.ProductId == productId).ToListAsync(ct);

        public async Task AddAsync(StockMovement movement, CancellationToken ct = default)
        {
            await _context.StockMovements.AddAsync(movement, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
