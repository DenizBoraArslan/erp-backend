using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly FinanceDbContext _context;

        public InvoiceRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id, ct);

        public async Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken ct = default)
            => await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .ToListAsync(ct);

        public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
            => await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .Where(i => i.CustomerId == customerId)
                .ToListAsync(ct);

        public async Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
            => await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.OrderId == orderId, ct);

        public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
        {
            await _context.Invoices.AddAsync(invoice, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
        {
            // If the invoice instance is already tracked in this DbContext (e.g. it was
            // loaded via GetByIdAsync earlier in the same request, mutated via a domain
            // method like UpdateDetails/Issue/AddPayment), its scalar property changes
            // (DueDate, Status, PaidAmount, UpdatedAt) are already picked up automatically
            // by the change tracker. Item replacement (clear + re-add in UpdateDetails) is
            // reconciled directly against the database below (a bulk delete that bypasses
            // change tracking entirely) since relying on EF Core's change tracker for this
            // private-backing-field/read-only-property collection navigation has proven
            // unreliable and produces a DbUpdateConcurrencyException on SaveChanges.
            if (_context.Entry(invoice).State != EntityState.Detached)
            {
                // See OrderRepository.UpdateAsync for the full explanation: the
                // originally-tracked InvoiceItem entries (loaded via Include) must be
                // fully detached before reconciling, otherwise EF Core's automatic
                // relationship fixup can reuse/conflate them with the brand-new
                // objects created by Invoice.UpdateDetails(), turning what should be a
                // delete+insert into a single UPDATE of the wrong row (0 rows
                // affected -> DbUpdateConcurrencyException).
                //
                // ExecuteDeleteAsync commits immediately and is NOT part of the same
                // unit of work as SaveChangesAsync, so both must be wrapped in one
                // explicit transaction - otherwise a failure in SaveChangesAsync would
                // leave the delete permanently applied even though the overall update
                // failed, silently corrupting the invoice.
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                var previouslyTrackedItems = _context.ChangeTracker.Entries<InvoiceItem>()
                    .Where(e => e.Entity.InvoiceId == invoice.Id)
                    .ToList();

                foreach (var trackedItem in previouslyTrackedItems)
                    trackedItem.State = EntityState.Detached;

                var newItems = invoice.Items.ToList();
                var newItemIds = newItems.Select(i => i.Id).ToHashSet();

                var existingIds = await _context.InvoiceItems
                    .AsNoTracking()
                    .Where(i => i.InvoiceId == invoice.Id)
                    .Select(i => i.Id)
                    .ToListAsync(ct);
                var existingIdSet = existingIds.ToHashSet();

                var idsToRemove = existingIds.Where(id => !newItemIds.Contains(id)).ToList();

                if (idsToRemove.Count > 0)
                {
                    await _context.InvoiceItems
                        .Where(i => idsToRemove.Contains(i.Id))
                        .ExecuteDeleteAsync(ct);
                }

                // Only genuinely new items (not already present in the DB) need
                // inserting. Status-only transitions like Issue don't touch the
                // item list at all, so their items' Ids already exist in the DB -
                // forcing EntityState.Added on them would attempt to INSERT rows
                // that already exist, causing a primary-key violation.
                foreach (var item in newItems)
                {
                    if (!existingIdSet.Contains(item.Id))
                        _context.Entry(item).State = EntityState.Added;
                }

                foreach (var payment in invoice.Payments)
                {
                    var paymentEntry = _context.Entry(payment);
                    if (paymentEntry.State == EntityState.Detached)
                        paymentEntry.State = EntityState.Added;
                }

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return;
            }

            var existing = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id, ct);

            if (existing is null) return;

            // Update scalar properties
            _context.Entry(existing).Property("Status").CurrentValue = invoice.Status;
            _context.Entry(existing).Property("PaidAmount").CurrentValue = invoice.PaidAmount;
            _context.Entry(existing).Property("UpdatedAt").CurrentValue = invoice.UpdatedAt;

            // Add new payments
            foreach (var payment in invoice.Payments)
            {
                var exists = existing.Payments.Any(p => p.Id == payment.Id);
                if (!exists)
                    await _context.Set<Payment>().AddAsync(payment, ct);
            }

            await _context.SaveChangesAsync(ct);
        }
        public async Task UpdatePaidAmountAsync(Guid invoiceId, decimal amount, CancellationToken ct = default)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId, ct);
            if (invoice is null) return;

            _context.Entry(invoice).Property("PaidAmount").CurrentValue =
                (decimal)_context.Entry(invoice).Property("PaidAmount").CurrentValue! + amount;

            var newPaidAmount = (decimal)_context.Entry(invoice).Property("PaidAmount").CurrentValue!;
            var totalAmount = (decimal)_context.Entry(invoice).Property("TotalAmount").CurrentValue!;

            _context.Entry(invoice).Property("Status").CurrentValue =
                newPaidAmount >= totalAmount
                    ? Finance.Domain.Enums.InvoiceStatus.Paid
                    : Finance.Domain.Enums.InvoiceStatus.PartiallyPaid;

            await _context.SaveChangesAsync(ct);
        }

        public async Task MarkAsPaidAsync(Guid invoiceId, CancellationToken ct = default)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId, ct);
            if (invoice is null) return;

            _context.Entry(invoice).Property("Status").CurrentValue = Finance.Domain.Enums.InvoiceStatus.Paid;
            _context.Entry(invoice).Property("PaidAmount").CurrentValue =
                _context.Entry(invoice).Property("TotalAmount").CurrentValue;

            await _context.SaveChangesAsync(ct);
        }
    }
}
