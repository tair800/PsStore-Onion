using Newtonsoft.Json;

namespace PsStore.Application.Exceptions
{
    public class ExceptionModel : ErrorStatusCode
    {
        public List<string> Errors { get; set; } = new();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ErrorStatusCode
    {
        public int StatusCode
        {
            get; set;
        }
    }
}
