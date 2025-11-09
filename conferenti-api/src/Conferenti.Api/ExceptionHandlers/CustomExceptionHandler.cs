using Conferenti.Api.Endpoints;
using Conferenti.Application.Endpoints;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Conferenti.Api.ExceptionHandlers;

//// https://www.milanjovanovic.tech/blog/problem-details-for-aspnetcore-apis?utm_source=newsletter&utm_medium=email&utm_campaign=tnw112

internal sealed class CustomExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = GetProblemDetails(exception, httpContext);

        if (problemDetails.Status != null)
        {
            httpContext.Response.StatusCode = problemDetails.Status.Value;
            // Log if Status500InternalServerError
            if (problemDetails.Status == StatusCodes.Status500InternalServerError)
            {

                logger.LogError(
                    exception,
                    " Handler: {HandlerName}" +
                    "\nRequest: {Request}" +
                    "\nExceptionType: {ExceptionType}" +
                    "\nExceptionMessage: {ExceptionMessage}" +
                    "\nStackTrace: {StackTrace}",
                    GetType().Name,
                    httpContext.Request.Path.ToString(),
                    exception.GetType().FullName,
                    exception.Message,
                    exception.StackTrace);
            }
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails GetProblemDetails(Exception exception, HttpContext httpContext)
    {
        var exceptionType = exception.GetType();
        return exception.ToProblemDetails(exceptionType == typeof(BadHttpRequestException) ? 400 : 500, httpContext);
    }
}
