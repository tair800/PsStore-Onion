namespace PsStore.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }  //Store query parameters
        public string Method { get; set; }  //Store HTTP method
        public string RequestBody { get; set; }  //Store request body
        public DateTime TimeStamp { get; set; }
    }
}
