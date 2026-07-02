using Inventory.Application.Common;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Categories.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var exists = await _categoryRepository.ExistsByNameAsync(request.Name, ct);
            if (exists)
                return Result<Guid>.Failure("A category with this name already exists.");

            var category = Category.Create(request.Name, request.Description);
            await _categoryRepository.AddAsync(category, ct);

            return Result<Guid>.Success(category.Id);
        }
    }
}
