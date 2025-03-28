using EPS.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPS.API.ActionFilters;

public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];

        var param = context.ActionArguments.SingleOrDefault(x => x.Value!.ToString()!.Contains("Dto")).Value
                    ?? throw new RequestObjectBadRequestException("A null request was sent");

        if (!context.ModelState.IsValid) throw new RequestObjectBadRequestException("Request sent is not valid");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}