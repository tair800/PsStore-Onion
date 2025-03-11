namespace PsStore.Application.Features.Dlc.Exceptions
{
    public class DlcNotFoundException : Exception
    {
        public DlcNotFoundException(int dlcId)
            : base($"No DLC found with ID {dlcId}.")
        {
        }
    }
}
