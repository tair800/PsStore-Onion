using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class DlcRules
{
    private readonly IUnitOfWork _unitOfWork;

    public DlcRules(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Result<Unit> PriceMustBeValid(decimal price)
    {
        if (price <= 0)
        {
            return Result<Unit>.Failure("Price must be greater than zero.", StatusCodes.Status400BadRequest, "INVALID_PRICE");
        }

        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result<Unit>> GameMustExist(int gameId)
    {
        var game = await _unitOfWork.GetReadRepository<Game>().GetAsync(g => g.Id == gameId);
        if (game == null)
        {
            return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
        }

        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result<Unit>> DlcNameMustBeUnique(int gameId, string name)
    {
        var dlc = await _unitOfWork.GetReadRepository<Dlc>().Find(d => d.GameId == gameId && d.Name == name).FirstOrDefaultAsync();
        if (dlc != null)
        {
            return Result<Unit>.Failure("DLC name must be unique.", StatusCodes.Status409Conflict, "DLC_NAME_ALREADY_EXISTS");
        }

        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result<Unit>> DlcMustExist(int dlcId)
    {
        var dlc = await _unitOfWork.GetReadRepository<Dlc>().GetAsync(d => d.Id == dlcId);
        if (dlc == null)
        {
            return Result<Unit>.Failure("DLC not found.", StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
        }

        return Result<Unit>.Success(Unit.Value);
    }

}
