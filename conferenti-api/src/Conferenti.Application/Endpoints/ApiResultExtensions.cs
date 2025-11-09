using Conferenti.Application.Helpers;
using Conferenti.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Conferenti.Application.Exceptions.ValidationException;

namespace Conferenti.Application.Endpoints;

public static class ApiResultExtensions
{
    public static ProblemDetails ToProblemDetails(this Error error, HttpContext? httpContext)
    {
        return new ProblemDetails
        {
            Type = Constants.HttpStatusCodeTypeDescription,
            Detail = error.Description,
            Status = GetStatusCode(error.Type),
            Title = GetTitle(error.Type),

            Instance = httpContext != null ? $"{httpContext.Request.Method} {httpContext.Request.Path}" : string.Empty,

            Extensions = new Dictionary<string, object?> { { "errors", new[] { error } } }
        };
    }

    public static ProblemDetails ToProblemDetails(this Exception exception, int statusCode, HttpContext? httpContext)
    {
        if (statusCode == 400)
        {
            return new ProblemDetails
            {
                Type = Constants.HttpStatusCodeTypeDescription,
                Detail = exception.Message,
                Status = GetStatusCode(ErrorType.Validation),
                Title = GetTitle(ErrorType.Validation),

                Instance = httpContext != null ? $"{httpContext.Request.Method} {httpContext.Request.Path}" : string.Empty,

                Extensions = new Dictionary<string, object?> { { "errors", new[] { new Error(ErrorType.Validation.ToString(), exception.Message, ErrorType.Validation) } } }
            };
        }

        return new ProblemDetails
        {
            Type = Constants.HttpStatusCodeTypeDescription,
            Detail = exception.Message,
            Status = GetStatusCode(ErrorType.Failure),
            Title = GetTitle(ErrorType.Failure),

            Instance = httpContext != null ? $"{httpContext.Request.Method} {httpContext.Request.Path}" : string.Empty,

            Extensions = new Dictionary<string, object?> { { "errors", new[] { new Error(ErrorType.Failure.ToString(), exception.Message, ErrorType.Failure) } } }
        };
    }

    public static ProblemDetails ToProblemDetails(this Exception exception, HttpContext? httpContext)
    {
        return new ProblemDetails
        {
            Type = Constants.HttpStatusCodeTypeDescription,
            Detail = exception.Message,
            Status = GetStatusCode(ErrorType.Failure),
            Title = GetTitle(ErrorType.Failure),

            Instance = httpContext != null ? $"{httpContext.Request.Method} {httpContext.Request.Path}" : string.Empty,

            Extensions = new Dictionary<string, object?> { { "errors", new[] { new Error(ErrorType.Failure.ToString(), exception.Message, ErrorType.Failure) } } }
        };
    }

    public static ProblemDetails ToProblemDetails(this ValidationException exception, HttpContext? httpContext)
    {
        return new ProblemDetails
        {
            Type = Constants.HttpStatusCodeTypeDescription,
            Detail = exception.Message,
            Status = GetStatusCode(ErrorType.Validation),
            Title = GetTitle(ErrorType.Validation),

            Instance = httpContext != null ? $"{httpContext.Request.Method} {httpContext.Request.Path}" : string.Empty,

            Extensions = new Dictionary<string, object?>
            {
                {
                    "errors",
                    new[]
                    {
                        exception.Errors.Select(e => new Error( "BadRequest", e.Description, ErrorType.Validation))
                    }
                }
            }
        };
    }

    private static int GetStatusCode(ErrorType errorType) =>
    errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Concurrency => StatusCodes.Status409Conflict,
        ErrorType.NoContent => StatusCodes.Status204NoContent,
        ErrorType.None => StatusCodes.Status200OK,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Concurrency => "Conflict",
            ErrorType.NoContent => "No Content",
            ErrorType.None => throw new NotImplementedException(),
            ErrorType.Failure => "Server Failure",
            _ => "Server Failure"
        };
}
