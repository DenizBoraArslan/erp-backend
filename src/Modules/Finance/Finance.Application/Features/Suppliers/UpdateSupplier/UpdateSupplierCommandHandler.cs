using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Suppliers.UpdateSupplier
{
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<bool>>
    {
        private readonly ISupplierRepository _supplierRepository;

        public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<bool>> Handle(UpdateSupplierCommand request, CancellationToken ct)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, ct);
            if (supplier is null)
                return Result<bool>.Failure("Tedarikçi bulunamadı.");

            supplier.Update(request.Name, request.ContactName, request.Phone, request.Address, request.TaxNumber);
            await _supplierRepository.UpdateAsync(supplier, ct);

            return Result<bool>.Success(true);
        }
    }
}
