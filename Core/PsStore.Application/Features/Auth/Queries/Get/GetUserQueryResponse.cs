namespace PsStore.Application.Features.Auth.Queries.Get
{
    public class GetUserQueryResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
        public DateTime CreatedDate { get; set; }


    }
}
