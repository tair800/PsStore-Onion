namespace PsStore.Application.Features.Dlc.Exceptions
{
    public class DlcCreationFailedException : Exception
    {
        public DlcCreationFailedException(string dlcName)
            : base($"Failed to create DLC '{dlcName}' due to an internal error.") { }
    }
}
