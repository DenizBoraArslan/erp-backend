using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly FinanceDbContext _context;

        public PaymentRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken ct = default)
            => await _context.Payments.Where(p => p.InvoiceId == invoiceId).ToListAsync(ct);

        public async Task AddAsync(Payment payment, CancellationToken ct = default)
        {
            await _context.Payments.AddAsync(payment, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
