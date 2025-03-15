namespace PsStore.Application.Features.Game.Exceptions
{
    public class PlatformNotFoundException : Exception
    {
        public PlatformNotFoundException(int platformId)
            : base($"Platform with ID '{platformId}' does not exist.") { }
    }
}
