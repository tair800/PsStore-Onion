using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting; // ✅ Correct namespace for IHostEnvironment
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Interfaces.Services;
using System.Text.Json;

namespace PsStore.Application.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IErrorLoggingService _errorLoggingService;
        private readonly IHostEnvironment _env; // ✅ Using IHostEnvironment instead of IWebHostEnvironment

        public ExceptionMiddleware(
            ILogger<ExceptionMiddleware> logger,
            IErrorLoggingService errorLoggingService,
            IHostEnvironment env)  // ✅ Changed IWebHostEnvironment to IHostEnvironment
        {
            _logger = logger;
            _errorLoggingService = errorLoggingService;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing request {Path}", httpContext.Request.Path);
                await _errorLoggingService.LogErrorToDatabase(httpContext, ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = StatusCodes.Status500InternalServerError;
            string errorCode = "INTERNAL_ERROR"; // Default error code

            if (exception is CategoryNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                errorCode = "CATEGORY_NOT_FOUND";
            }

            var response = new
            {
                success = false,
                errorCode,
                message = exception.Message,
                statusCode,
                timestamp = DateTime.UtcNow,
                stackTrace = _env.IsDevelopment() ? exception.StackTrace : null  // ✅ Add stackTrace only in Development
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            // ✅ Use JsonSerializer instead of WriteAsJsonAsync
            await JsonSerializer.SerializeAsync(context.Response.Body, response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
