using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class GameRules
{
    private readonly IUnitOfWork _unitOfWork;

    public GameRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> GameTitleMustBeUnique(string title)
    {
        var games = await _unitOfWork.GetReadRepository<Game>().GetAllAsync(
            predicate: g => g.Title == title);

        if (games.Any())
        {
            return Result<Unit>.Failure("Game title must be unique.", StatusCodes.Status400BadRequest, "TITLE_ALREADY_EXISTS");
        }

        return Result<Unit>.Success(Unit.Value);
    }

    public Task<Result<Unit>> PlatformMustExist(int platformId)
    {
        bool exists = Enum.IsDefined(typeof(Platform), platformId);

        if (!exists)
        {
            return Task.FromResult(Result<Unit>.Failure(
                $"Platform with ID {platformId} does not exist.",
                StatusCodes.Status400BadRequest,
                "PLATFORM_NOT_FOUND"
            ));
        }

        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }

    public async Task<Result<Unit>> CategoryMustExist(int categoryId)
    {
        var category = await _unitOfWork.GetReadRepository<Category>().GetAsync(c => c.Id == categoryId);
        if (category == null)
        {
            return Result<Unit>.Failure("Category not found.", StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND");
        }

        return Result<Unit>.Success(Unit.Value);
    }


    public async Task<Result<Unit>> GameMustExist(int gameId)
    {
        var game = await _unitOfWork.GetReadRepository<Game>().Find(g => g.Id == gameId).FirstOrDefaultAsync();

        if (game == null)
        {
            return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
