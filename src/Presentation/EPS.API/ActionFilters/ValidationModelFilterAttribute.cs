using EPS.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPS.API.ActionFilters;

public class ValidationModelFilterAttribute<TModel> : IAsyncActionFilter where TModel : class
{
    private readonly IValidator<TModel> _validator;

    public ValidationModelFilterAttribute(IValidator<TModel> validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ActionArguments.TryGetValue("request", out var request) && request is TModel model)
        {
            var validationResult = await _validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var response = new GenericResponse<string>
                {
                    StatusCode = 400,
                    Message = validationResult.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                };
                context.Result = new BadRequestObjectResult(response);
                return;
            }
        }

        await next();
    }
}