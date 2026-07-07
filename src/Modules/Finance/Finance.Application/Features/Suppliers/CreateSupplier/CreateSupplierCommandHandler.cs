using Finance.Application.Common;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Suppliers.CreateSupplier
{
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<Guid>>
    {
        private readonly ISupplierRepository _supplierRepository;

        public CreateSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<Guid>.Failure("Tedarikçi adı zorunludur.");

            var supplier = Supplier.Create(
                request.Name,
                request.ContactName,
                request.Email,
                request.Phone,
                request.Address,
                request.TaxNumber
            );

            await _supplierRepository.AddAsync(supplier, ct);

            return Result<Guid>.Success(supplier.Id);
        }
    }
}
