using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;

namespace PsStore.Application.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            int statusCode = GetStatusCode(exception);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            var response = new ExceptionModel
            {
                StatusCode = statusCode
            };

            if (exception is ValidationException validationException)
            {
                response.Errors = validationException.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}") // ✅ Include property names
                    .ToList();
            }
            else
            {
                response.Errors.Add($"Error Message: {exception.Message}"); // ✅ Now works since `Errors` is initialized
            }

            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound, // ✅ Fixed NotFound to return 404
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
