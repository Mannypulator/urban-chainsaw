using EPS.Application.Validators;
using FluentValidation;

namespace EPS.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
        });

        services.AddValidatorsFromAssemblyContaining<CreateContributionDtoValidator>();

        return services;
    }
}