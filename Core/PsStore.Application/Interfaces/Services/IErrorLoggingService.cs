using Microsoft.AspNetCore.Http;

namespace PsStore.Application.Interfaces.Services
{
    public interface IErrorLoggingService
    {
        Task LogErrorToDatabase(HttpContext httpContext, Exception exception);
    }
}
