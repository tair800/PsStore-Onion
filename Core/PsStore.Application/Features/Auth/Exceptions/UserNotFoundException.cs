namespace PsStore.Application.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email)
            : base($"User not found for email: {email}") { }
    }
}
