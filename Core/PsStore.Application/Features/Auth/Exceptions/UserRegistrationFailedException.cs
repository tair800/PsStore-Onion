namespace PsStore.Application.Exceptions
{
    public class UserRegistrationFailedException : Exception
    {
        public UserRegistrationFailedException(string email)
            : base($"Registration failed for email: {email}") { }
    }
}
