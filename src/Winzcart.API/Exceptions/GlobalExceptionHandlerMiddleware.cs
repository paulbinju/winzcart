using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Winzcart.API.Exceptions;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode = StatusCodes.Status500InternalServerError;
        string title = "An unexpected error occurred.";
        
        switch (exception)
        {
            case Domain.Exceptions.NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                title = "Not Found";
                break;
            case Domain.Exceptions.UnauthorizedException unauthEx:
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Unauthorized";
                break;
            case Domain.Exceptions.DomainException domainEx:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
