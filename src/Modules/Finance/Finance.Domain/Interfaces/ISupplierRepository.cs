using Finance.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Domain.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Supplier supplier, CancellationToken ct = default);
        Task UpdateAsync(Supplier supplier, CancellationToken ct = default);
    }
}
