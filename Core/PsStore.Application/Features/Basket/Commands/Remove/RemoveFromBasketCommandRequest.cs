using MediatR;

public class RemoveFromBasketCommandRequest : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
    public int GameId { get; set; }
}
