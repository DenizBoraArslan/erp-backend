using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Suppliers.DeactivateSupplier
{
    public class DeactivateSupplierCommandHandler : IRequestHandler<DeactivateSupplierCommand, Result<bool>>
    {
        private readonly ISupplierRepository _supplierRepository;

        public DeactivateSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<bool>> Handle(DeactivateSupplierCommand request, CancellationToken ct)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, ct);
            if (supplier is null)
                return Result<bool>.Failure("Tedarikçi bulunamadı.");

            supplier.Deactivate();
            await _supplierRepository.UpdateAsync(supplier, ct);

            return Result<bool>.Success(true);
        }
    }
}
