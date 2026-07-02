using Inventory.Application.Common;
using Inventory.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Categories.ActivateCategory
{
    public class ActivateCategoryCommandHandler : IRequestHandler<ActivateCategoryCommand, Result<bool>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public ActivateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<bool>> Handle(ActivateCategoryCommand request, CancellationToken ct)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, ct);
            if (category is null)
                return Result<bool>.Failure("Category not found.");

            category.Activate();
            await _categoryRepository.UpdateAsync(category, ct);

            return Result<bool>.Success(true);
        }
    }
}
