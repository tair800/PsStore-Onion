using Microsoft.EntityFrameworkCore;
using PsStore.Application.Features.Category.Exceptions;
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


        public async Task CategoryMustExist(int categoryId)
        {
            var categoryExists = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .AnyAsync(c => c.Id == categoryId);

            if (!categoryExists)
                throw new CategoryNotFoundException(categoryId);

        }


        public async Task CategoryNameMustBeUnique(string categoryName)
        {
            var existingCategory = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .AnyAsync(c => c.Name == categoryName);

            if (existingCategory)
                throw new CategoryAlreadyExistsException(categoryName);

        }


        public async Task CategoryMustHaveAtLeastOneGame(int categoryId)
        {
            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .GetAsync(c => c.Id == categoryId, include: q => q.Include(c => c.Games));

            if (category == null || category.Games == null || !category.Games.Any())
                throw new CategoryMustHaveGamesException(categoryId);

        }


        public void CategoryNameMustBeValid(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty.");


            if (categoryName.Length < 3 || categoryName.Length > 100)
                throw new ArgumentException("Category name must be between 3 and 100 characters.");

        }
    }
}
