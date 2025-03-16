using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Features.Game.Exceptions;
using PsStore.Application.Interfaces.Services;
using System.Text.Json;

namespace PsStore.Application.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IErrorLoggingService _errorLoggingService;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            ILogger<ExceptionMiddleware> logger,
            IErrorLoggingService errorLoggingService,
            IHostEnvironment env)
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
            string errorCode = "INTERNAL_ERROR";

            switch (exception)
            {
                // Category Exceptions
                case CategoryNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorCode = "CATEGORY_NOT_FOUND";
                    break;
                case CategoryAlreadyExistsException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "CATEGORY_ALREADY_EXISTS";
                    break;
                case CategoryAlreadyDeletedException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "CATEGORY_ALREADY_DELETED";
                    break;
                case CategoryAlreadyActiveException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "CATEGORY_ALREADY_ACTIVE";
                    break;
                case CategoryCannotBeDeletedException:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorCode = "CATEGORY_CANNOT_BE_DELETED";
                    break;
                case CategoryCreationFailedException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "CATEGORY_CREATION_FAILED";
                    break;
                case CategoryDeleteFailedException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "CATEGORY_DELETE_FAILED";
                    break;
                case CategoryMustHaveGamesException:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorCode = "CATEGORY_MUST_HAVE_GAMES";
                    break;

                // Game Exceptions
                case GameNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorCode = "GAME_NOT_FOUND";
                    break;
                case GameTitleMustBeUniqueException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "GAME_TITLE_MUST_BE_UNIQUE";
                    break;
                case GameCreationFailedException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "GAME_CREATION_FAILED";
                    break;
                case GameUpdateFailedException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "GAME_UPDATE_FAILED";
                    break;
                case GameAlreadyActiveException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "GAME_ALREADY_ACTIVE";
                    break;
                case GameAlreadyDeletedException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "GAME_ALREADY_DELETED";
                    break;
                case GameNotDeletedException:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorCode = "GAME_NOT_DELETED";
                    break;

                // Platform Exceptions
                case PlatformNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorCode = "PLATFORM_NOT_FOUND";
                    break;

                // DLC Exceptions
                case DlcNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorCode = "DLC_NOT_FOUND";
                    break;
                case DlcAlreadyExistsException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "DLC_ALREADY_EXISTS";
                    break;
                case DlcAlreadyDeletedException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "DLC_ALREADY_DELETED";
                    break;
                case DlcAlreadyActiveException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "DLC_ALREADY_ACTIVE";
                    break;
                case DlcCreationFailedException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "DLC_CREATION_FAILED";
                    break;
                case DlcUpdateFailedException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "DLC_UPDATE_FAILED";
                    break;

                // Default Case for Unknown Exceptions
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "INTERNAL_ERROR";
                    break;
            }

            var response = new
            {
                success = false,
                errorCode,
                message = exception.Message,
                statusCode,
                timestamp = DateTime.UtcNow,
                stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await JsonSerializer.SerializeAsync(context.Response.Body, response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
