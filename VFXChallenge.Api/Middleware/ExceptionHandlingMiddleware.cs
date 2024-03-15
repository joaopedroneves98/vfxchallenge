namespace VFXChallenge.Api.Middleware
{
    using Infrastructure.Exceptions;

    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;

    public class ExceptionHandlingMiddleware : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            this._logger = logger;
        }
        
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            this._logger.LogError(
                exception, "Exception occurred: {Message}", exception.Message);
            
            var response = exception switch
            {
                NotFoundException _ => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found.",
                    Detail = exception.Message
                },
                ValidationException _ => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation error.",
                    Detail = exception.Message
                },
                ArgumentException _ => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid argument.",
                    Detail = exception.Message
                },
                ApplicationErrorException _ => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Application exception occurred.",
                    Detail = exception.Message
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal server error. Please try again later.",
                    Detail = exception.Message
                }
            };

            httpContext.Response.StatusCode = response.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(response, cancellationToken);

            return true;

        }
    }
}