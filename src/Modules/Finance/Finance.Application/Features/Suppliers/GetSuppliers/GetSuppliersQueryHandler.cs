using Finance.Application.Common;
using Finance.Application.DTOs;
using Finance.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Suppliers.GetSuppliers
{
    public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, Result<IEnumerable<SupplierDto>>>
    {
        private readonly ISupplierRepository _supplierRepository;

        public GetSuppliersQueryHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<IEnumerable<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken ct)
        {
            var suppliers = await _supplierRepository.GetAllAsync(ct);

            var dtos = suppliers.Select(s => new SupplierDto(
                s.Id, s.Name, s.ContactName, s.Email, s.Phone, s.Address, s.TaxNumber, s.IsActive, s.CreatedAt
            ));

            return Result<IEnumerable<SupplierDto>>.Success(dtos);
        }
    }
}
