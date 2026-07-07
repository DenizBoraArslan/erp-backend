using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Persistence.Repositories
{
    // Mirrors InvoiceRepository's item/payment reconciliation pattern exactly
    // (see that class for the full rationale) — PurchaseInvoiceItems must be
    // detached-then-bulk-deleted-then-re-added rather than relying on EF
    // Core's change tracker for the private-backing-field collection nav,
    // otherwise UpdateDetails() produces a DbUpdateConcurrencyException.
    public class PurchaseInvoiceRepository : IPurchaseInvoiceRepository
    {
        private readonly FinanceDbContext _context;

        public PurchaseInvoiceRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.PurchaseInvoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id, ct);

        public async Task<IEnumerable<PurchaseInvoice>> GetAllAsync(CancellationToken ct = default)
            => await _context.PurchaseInvoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .ToListAsync(ct);

        public async Task<IEnumerable<PurchaseInvoice>> GetBySupplierIdAsync(Guid supplierId, CancellationToken ct = default)
            => await _context.PurchaseInvoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .Where(i => i.SupplierId == supplierId)
                .ToListAsync(ct);

        public async Task AddAsync(PurchaseInvoice purchaseInvoice, CancellationToken ct = default)
        {
            await _context.PurchaseInvoices.AddAsync(purchaseInvoice, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(PurchaseInvoice purchaseInvoice, CancellationToken ct = default)
        {
            if (_context.Entry(purchaseInvoice).State != EntityState.Detached)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                var previouslyTrackedItems = _context.ChangeTracker.Entries<PurchaseInvoiceItem>()
                    .Where(e => e.Entity.PurchaseInvoiceId == purchaseInvoice.Id)
                    .ToList();

                foreach (var trackedItem in previouslyTrackedItems)
                    trackedItem.State = EntityState.Detached;

                var newItems = purchaseInvoice.Items.ToList();
                var newItemIds = newItems.Select(i => i.Id).ToHashSet();

                var existingIds = await _context.PurchaseInvoiceItems
                    .AsNoTracking()
                    .Where(i => i.PurchaseInvoiceId == purchaseInvoice.Id)
                    .Select(i => i.Id)
                    .ToListAsync(ct);
                var existingIdSet = existingIds.ToHashSet();

                var idsToRemove = existingIds.Where(id => !newItemIds.Contains(id)).ToList();

                if (idsToRemove.Count > 0)
                {
                    await _context.PurchaseInvoiceItems
                        .Where(i => idsToRemove.Contains(i.Id))
                        .ExecuteDeleteAsync(ct);
                }

                // Only genuinely new items (not already present in the DB) need
                // inserting. Status-only transitions like Confirm don't touch the
                // item list at all, so their items' Ids already exist in the DB -
                // forcing EntityState.Added on them would attempt to INSERT rows
                // that already exist, causing a primary-key violation.
                foreach (var item in newItems)
                {
                    if (!existingIdSet.Contains(item.Id))
                        _context.Entry(item).State = EntityState.Added;
                }

                foreach (var payment in purchaseInvoice.Payments)
                {
                    var paymentEntry = _context.Entry(payment);
                    if (paymentEntry.State == EntityState.Detached)
                        paymentEntry.State = EntityState.Added;
                }

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return;
            }

            var existing = await _context.PurchaseInvoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == purchaseInvoice.Id, ct);

            if (existing is null) return;

            _context.Entry(existing).Property("Status").CurrentValue = purchaseInvoice.Status;
            _context.Entry(existing).Property("PaidAmount").CurrentValue = purchaseInvoice.PaidAmount;
            _context.Entry(existing).Property("UpdatedAt").CurrentValue = purchaseInvoice.UpdatedAt;

            foreach (var payment in purchaseInvoice.Payments)
            {
                var exists = existing.Payments.Any(p => p.Id == payment.Id);
                if (!exists)
                    await _context.Set<PurchasePayment>().AddAsync(payment, ct);
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdatePaidAmountAsync(Guid purchaseInvoiceId, decimal amount, CancellationToken ct = default)
        {
            var invoice = await _context.PurchaseInvoices.FirstOrDefaultAsync(i => i.Id == purchaseInvoiceId, ct);
            if (invoice is null) return;

            _context.Entry(invoice).Property("PaidAmount").CurrentValue =
                (decimal)_context.Entry(invoice).Property("PaidAmount").CurrentValue! + amount;

            var newPaidAmount = (decimal)_context.Entry(invoice).Property("PaidAmount").CurrentValue!;
            var totalAmount = (decimal)_context.Entry(invoice).Property("TotalAmount").CurrentValue!;

            _context.Entry(invoice).Property("Status").CurrentValue =
                newPaidAmount >= totalAmount
                    ? Finance.Domain.Enums.PurchaseInvoiceStatus.Paid
                    : Finance.Domain.Enums.PurchaseInvoiceStatus.PartiallyPaid;

            await _context.SaveChangesAsync(ct);
        }

        public async Task MarkAsPaidAsync(Guid purchaseInvoiceId, CancellationToken ct = default)
        {
            var invoice = await _context.PurchaseInvoices.FirstOrDefaultAsync(i => i.Id == purchaseInvoiceId, ct);
            if (invoice is null) return;

            _context.Entry(invoice).Property("Status").CurrentValue = Finance.Domain.Enums.PurchaseInvoiceStatus.Paid;
            _context.Entry(invoice).Property("PaidAmount").CurrentValue =
                _context.Entry(invoice).Property("TotalAmount").CurrentValue;

            await _context.SaveChangesAsync(ct);
        }
    }
}
