namespace PsStore.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Method { get; set; }
        public string RequestBody { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
