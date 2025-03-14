using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.Services;
using PsStore.Domain.Entities;
using PsStore.Persistance.Context;
using System.Text;

namespace PsStore.Infrastructure.Services
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ErrorLoggingService> _logger;

        public ErrorLoggingService(IServiceProvider serviceProvider, ILogger<ErrorLoggingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task LogErrorToDatabase(HttpContext httpContext, Exception exception)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Capture request body
                string requestBody = await GetRequestBodyAsync(httpContext);

                var errorLog = new ErrorLog
                {
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Path = httpContext.Request.Path,
                    TimeStamp = DateTime.UtcNow,
                    RequestBody = requestBody,  // ✅ Save request body
                    QueryString = httpContext.Request.QueryString.ToString(),  // ✅ Save query params
                    Method = httpContext.Request.Method // ✅ Save HTTP method
                };

                dbContext.ErrorLogs.Add(errorLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Failed to log error to the database.");
            }
        }

        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();  // ✅ Enable request body reading multiple times

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // ✅ Reset body position after reading

            return string.IsNullOrWhiteSpace(body) ? "No body" : body;
        }
    }
}
