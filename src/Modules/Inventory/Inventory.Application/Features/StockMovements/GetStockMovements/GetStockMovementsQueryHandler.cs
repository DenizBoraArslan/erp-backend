using Inventory.Application.Common;
using Inventory.Application.DTOs;
using Inventory.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Application.Features.StockMovements.GetStockMovements
{
    public class GetStockMovementsQueryHandler : IRequestHandler<GetStockMovementsQuery, Result<IEnumerable<StockMovementDto>>>
    {
        private readonly IStockMovementRepository _stockMovementRepository;

        public GetStockMovementsQueryHandler(IStockMovementRepository stockMovementRepository)
        {
            _stockMovementRepository = stockMovementRepository;
        }

        public async Task<Result<IEnumerable<StockMovementDto>>> Handle(GetStockMovementsQuery request, CancellationToken ct)
        {
            var movements = request.ProductId.HasValue
                ? await _stockMovementRepository.GetByProductIdAsync(request.ProductId.Value, ct)
                : await _stockMovementRepository.GetAllAsync(ct);

            var dtos = movements.Select(m => new StockMovementDto(
                m.Id,
                m.ProductId,
                m.Product?.Name ?? string.Empty,
                m.Quantity,
                m.Type.ToString(),
                m.Note,
                m.CreatedAt
            ));

            return Result<IEnumerable<StockMovementDto>>.Success(dtos);
        }
    }
}
