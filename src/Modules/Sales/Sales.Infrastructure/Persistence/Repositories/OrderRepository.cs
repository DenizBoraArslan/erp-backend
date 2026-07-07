using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SalesDbContext _context;

        public OrderRepository(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
            => await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToListAsync(ct);

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
            => await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync(ct);

        public async Task AddAsync(Order order, CancellationToken ct = default)
        {
            await _context.Orders.AddAsync(order, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            var entry = _context.Entry(order);

            if (entry.State == EntityState.Detached)
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync(ct);
                return;
            }

            // The order was already loaded/tracked in this DbContext (e.g. via
            // GetByIdAsync earlier in the same request, which also Included the
            // original OrderItems). Domain methods like UpdateDetails() replace the
            // in-memory OrderItems collection (clear + re-add with brand new objects),
            // but if the ORIGINAL tracked item entries are left attached, EF Core's
            // automatic relationship fixup ends up reusing/conflating them with the
            // new objects - resulting in the old row being UPDATEd with the new row's
            // data (instead of deleted) and the "new" row never actually being
            // inserted, which then fails as a 0-rows-affected concurrency exception.
            // Fully detaching the old entries first removes any such ambiguity.
            //
            // ExecuteDeleteAsync runs and commits immediately - it does NOT
            // participate in the same unit of work as SaveChangesAsync. Without an
            // explicit transaction wrapping both, a failure in the SaveChangesAsync
            // call below would leave the delete permanently applied even though the
            // overall update failed, silently corrupting the order (fewer items than
            // it should have). Wrap everything in one transaction so it's all-or-nothing.
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            var previouslyTrackedItems = _context.ChangeTracker.Entries<OrderItem>()
                .Where(e => e.Entity.OrderId == order.Id)
                .ToList();

            foreach (var trackedItem in previouslyTrackedItems)
                trackedItem.State = EntityState.Detached;

            var newItems = order.OrderItems.ToList();
            var newItemIds = newItems.Select(i => i.Id).ToHashSet();

            var existingIds = await _context.OrderItems
                .AsNoTracking()
                .Where(i => i.OrderId == order.Id)
                .Select(i => i.Id)
                .ToListAsync(ct);
            var existingIdSet = existingIds.ToHashSet();

            var idsToRemove = existingIds.Where(id => !newItemIds.Contains(id)).ToList();

            if (idsToRemove.Count > 0)
            {
                await _context.OrderItems
                    .Where(i => idsToRemove.Contains(i.Id))
                    .ExecuteDeleteAsync(ct);
            }

            // Only genuinely new items (not already present in the DB) need
            // inserting. Status-only transitions like Confirm/Ship/Deliver don't
            // touch the item list at all, so their items' Ids already exist in
            // the DB - forcing EntityState.Added on them would attempt to INSERT
            // rows that already exist, causing a primary-key violation.
            foreach (var item in newItems)
            {
                if (!existingIdSet.Contains(item.Id))
                    _context.Entry(item).State = EntityState.Added;
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
    }
}
