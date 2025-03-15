namespace PsStore.Application.Features.Dlc.Exceptions
{
    public class DlcUpdateFailedException : Exception
    {
        public DlcUpdateFailedException(int dlcId)
            : base($"Failed to update DLC with ID '{dlcId}' due to an internal error.") { }
    }
}
