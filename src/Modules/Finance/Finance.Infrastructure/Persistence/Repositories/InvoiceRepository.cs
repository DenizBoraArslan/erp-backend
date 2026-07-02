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

        public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
        {
            await _context.Invoices.AddAsync(invoice, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
        {
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
