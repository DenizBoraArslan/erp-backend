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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly InventoryDbContext _context;

        public CategoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
            => await _context.Categories.ToListAsync(ct);

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
            => await _context.Categories.AnyAsync(c => c.Name == name, ct);

        public async Task AddAsync(Category category, CancellationToken ct = default)
        {
            await _context.Categories.AddAsync(category, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Category category, CancellationToken ct = default)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(ct);
        }
    }
}
