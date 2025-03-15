public class DlcNotFoundException : Exception
{
    public DlcNotFoundException()
        : base("One or more specified DLCs were not found.") { }
}
