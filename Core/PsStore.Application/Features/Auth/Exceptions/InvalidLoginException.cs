namespace PsStore.Application.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException(string email)
            : base($"Invalid login attempt for email: {email}") { }
    }
}
