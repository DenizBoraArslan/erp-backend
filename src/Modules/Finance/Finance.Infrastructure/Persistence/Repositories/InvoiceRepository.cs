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
          
            if (_context.Entry(invoice).State != EntityState.Detached)
            {
     
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

            _context.Entry(existing).Property("Status").CurrentValue = invoice.Status;
            _context.Entry(existing).Property("PaidAmount").CurrentValue = invoice.PaidAmount;
            _context.Entry(existing).Property("UpdatedAt").CurrentValue = invoice.UpdatedAt;

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
