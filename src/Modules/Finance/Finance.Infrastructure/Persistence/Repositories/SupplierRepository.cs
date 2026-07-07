using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Persistence.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly FinanceDbContext _context;

        public SupplierRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken ct = default)
            => await _context.Suppliers.ToListAsync(ct);

        public async Task AddAsync(Supplier supplier, CancellationToken ct = default)
        {
            await _context.Suppliers.AddAsync(supplier, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Supplier supplier, CancellationToken ct = default)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync(ct);
        }
    }
}
