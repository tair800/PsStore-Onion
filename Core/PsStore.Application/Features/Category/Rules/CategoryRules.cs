using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Rules
{
    public class CategoryRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Result<Unit> CategoryNameMustBeValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Unit>.Failure("Category name cannot be empty.", StatusCodes.Status400BadRequest, "CATEGORY_NAME_EMPTY");
            }

            if (name.Length < 3 || name.Length > 255)
            {
                return Result<Unit>.Failure("Category name must be between 3 and 255 characters.", StatusCodes.Status400BadRequest, "CATEGORY_NAME_INVALID_LENGTH");
            }

            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<Unit>> CategoryNameMustBeUnique(string name)
        {
            var existingCategory = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().Find(c => c.Name == name).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                return Result<Unit>.Failure("Category name must be unique.", StatusCodes.Status409Conflict, "CATEGORY_NAME_ALREADY_EXISTS");
            }

            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<Unit>> CategoryMustExist(int categoryId)
        {
            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAsync(c => c.Id == categoryId);
            if (category == null)
            {
                return Result<Unit>.Failure("Category not found.", StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
