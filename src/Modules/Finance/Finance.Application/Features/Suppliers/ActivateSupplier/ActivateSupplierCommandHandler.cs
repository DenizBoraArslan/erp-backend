using Finance.Application.Common;
using Finance.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Application.Features.Suppliers.ActivateSupplier
{
    public class ActivateSupplierCommandHandler : IRequestHandler<ActivateSupplierCommand, Result<bool>>
    {
        private readonly ISupplierRepository _supplierRepository;

        public ActivateSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<bool>> Handle(ActivateSupplierCommand request, CancellationToken ct)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, ct);
            if (supplier is null)
                return Result<bool>.Failure("Tedarikçi bulunamadı.");

            supplier.Activate();
            await _supplierRepository.UpdateAsync(supplier, ct);

            return Result<bool>.Success(true);
        }
    }
}
