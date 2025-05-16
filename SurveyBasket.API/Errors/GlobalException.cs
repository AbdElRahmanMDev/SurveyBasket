using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.API.Errors;

public class GlobalException : IExceptionHandler
{
    private readonly ILogger<GlobalException> _logger;

    public GlobalException(ILogger<GlobalException> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Something went Wrong : {Message}", exception.Message);

        var problem = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#page-63"
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem);

        return true;
    }
}
