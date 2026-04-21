using MediatR;
using Common.Results;
using Common.Interfaces;
using Purchase.Domain.Entities;

namespace Purchase.Application.Features.Suppliers.Commands;

public class CreateSupplierCommand : IRequest<Result<CreateSupplierResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public int Type { get; set; }
}

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<CreateSupplierResponse>>
{
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierCommandHandler(IRepository<Supplier> supplierRepository, IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateSupplierResponse>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                Type = (SupplierType)request.Type,
                TaxId = request.TaxId,
                IsActive = true
            };

            await _supplierRepository.AddAsync(supplier, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateSupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name
            };

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<CreateSupplierResponse>("Unexpected error occurred while creating supplier.");
        }
    }
}

public class CreateSupplierResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
