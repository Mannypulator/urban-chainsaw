using EPS.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EPS.API;

public class GlobalExceptionalHandler(ILogger<GlobalExceptionalHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error"
        };

        if (exception is BadRequestException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = exception.Message;
        }
        
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

       

       httpContext.Response.StatusCode = problemDetails.Status.Value;
       
       await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
       return true;
    }
}