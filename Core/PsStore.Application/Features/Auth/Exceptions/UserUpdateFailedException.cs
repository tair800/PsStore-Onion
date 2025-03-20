namespace PsStore.Application.Exceptions
{
    public class UserUpdateFailedException : Exception
    {
        public UserUpdateFailedException(string userId)
            : base($"Failed to update user details for user ID: {userId}") { }
    }
}
