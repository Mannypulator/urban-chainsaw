using EPS.Domain.Repositories;
using EPS.Persistence.Data;
using EPS.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPS.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EPSDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(EPSDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IEmployerRepository, EmployerRepository>();
        services.AddScoped<IContributionRepository, ContributionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}