using Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Interfaces
{
    public interface IStockMovementRepository
    {
        Task<IEnumerable<StockMovement>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
        Task<IEnumerable<StockMovement>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(StockMovement movement, CancellationToken ct = default);
    }
}
