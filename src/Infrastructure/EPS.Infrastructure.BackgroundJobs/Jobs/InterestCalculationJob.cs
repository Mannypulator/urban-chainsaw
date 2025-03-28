using EPS.Application.Services;
using EPS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EPS.Infrastructure.BackgroundJobs.Jobs;

public class InterestCalculationJob
{
    private readonly IContributionService _contributionService;
    private readonly ILogger<InterestCalculationJob> _logger;

    public InterestCalculationJob(
        IContributionService contributionService,
        ILogger<InterestCalculationJob> logger)
    {
        _contributionService = contributionService;
        _logger = logger;
    }

    public async Task CalculateInterestForProcessedContributionsAsync()
    {
        try
        {
            _logger.LogInformation("Starting interest calculation for processed contributions...");

            var processedContributions =
                await _contributionService.GetContributionsByStatusAsync(ContributionStatus.Processed);
            foreach (var contribution in processedContributions)
                try
                {
                    // Skip contributions that already have interest calculated
                    if (contribution.InterestCalculationDate.HasValue) continue;

                    await _contributionService.CalculateInterestAsync(contribution.Id);
                    _logger.LogInformation("Successfully calculated interest for contribution {ContributionId}",
                        contribution.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating interest for contribution {ContributionId}",
                        contribution.Id);
                }

            _logger.LogInformation("Finished calculating interest for processed contributions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in interest calculation job");
            throw;
        }
    }
}