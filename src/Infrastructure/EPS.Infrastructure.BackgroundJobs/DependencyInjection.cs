using EPS.Infrastructure.BackgroundJobs.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Hangfire.Dashboard;

namespace EPS.Infrastructure.BackgroundJobs;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Hangfire
        services.AddHangfire((serviceProvider, config) => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        // Add Hangfire server
        services.AddHangfireServer();

        // Register background jobs
        services.AddScoped<BenefitEligibilityJob>();
        services.AddScoped<ContributionProcessingJob>();
        services.AddScoped<InterestCalculationJob>();

        return services;
    }

    public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder app)
    {
        // Configure recurring jobs
        var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        // Process contributions every hour
        recurringJobManager.AddOrUpdate(
            "ProcessContributions",
            Hangfire.Common.Job.FromExpression<ContributionProcessingJob>(x => x.ProcessPendingContributionsAsync()),
            Cron.Hourly());

        // Calculate interest daily at midnight
        recurringJobManager.AddOrUpdate(
            "CalculateInterest",
            Hangfire.Common.Job.FromExpression<InterestCalculationJob>(x => x.CalculateInterestForProcessedContributionsAsync()),
            Cron.Daily());

        // Update benefit eligibility daily at 1 AM
        recurringJobManager.AddOrUpdate(
            "UpdateBenefitEligibility",
            Hangfire.Common.Job.FromExpression<BenefitEligibilityJob>(x => x.UpdateBenefitEligibilityForAllMembersAsync()),
            Cron.Daily(1));

        // Enable Hangfire Dashboard
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new AllowAllConnectionsFilter() }
        });

        return app;
    }
}

public class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}