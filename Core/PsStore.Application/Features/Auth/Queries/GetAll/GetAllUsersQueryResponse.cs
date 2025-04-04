namespace PsStore.Application.Features.Auth.Queries.GetAll
{
    public class GetAllUsersQueryResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
