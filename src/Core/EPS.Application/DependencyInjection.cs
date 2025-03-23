using EPS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EPS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // Register application services
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IEmployerService, EmployerService>();
        services.AddScoped<IContributionService, ContributionService>();

        return services;
    }
}