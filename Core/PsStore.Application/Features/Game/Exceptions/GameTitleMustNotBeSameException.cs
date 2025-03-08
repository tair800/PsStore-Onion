namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameTitleMustNotBeSameException : Exception
    {
        public GameTitleMustNotBeSameException()
            : base("A game with the same title already exists.") { }
    }
}
