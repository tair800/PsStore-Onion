using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Exceptions;

namespace PsStore.Application.Features.Game.Rules
{
    public class GameRules : BaseRules
    {
        public Task GameTitleMustNotBeSame(IList<Domain.Entities.Game> games, string requestTitle)
        {
            if (games.Any(x => x.Title == requestTitle))
                throw new GameTitleMustNotBeSameException();

            return Task.CompletedTask;
        }
    }
}
