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
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _context;

        public ProductRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default)
            => await _context.Products.Include(p => p.Category).Where(p => p.IsActive).ToListAsync(ct);

        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
            => await _context.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId && p.IsActive).ToListAsync(ct);

        public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken ct = default)
            => await _context.Products.AnyAsync(p => p.SKU == sku, ct);

        public async Task AddAsync(Product product, CancellationToken ct = default)
        {
            await _context.Products.AddAsync(product, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Product product, CancellationToken ct = default)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(ct);
        }
    }
}
