using EPS.Application.Services;
using EPS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EPS.Infrastructure.BackgroundJobs.Jobs;

public class ContributionProcessingJob
{
    private readonly IContributionService _contributionService;
    private readonly ILogger<ContributionProcessingJob> _logger;

    public ContributionProcessingJob(
        IContributionService contributionService,
        ILogger<ContributionProcessingJob> logger)
    {
        _contributionService = contributionService;
        _logger = logger;
    }

    public async Task ProcessPendingContributionsAsync()
    {
        try
        {
            _logger.LogInformation("Starting to process pending contributions...");

            var pendingContributions =
                await _contributionService.GetContributionsByStatusAsync(ContributionStatus.Pending);
            foreach (var contribution in pendingContributions)
                try
                {
                    await _contributionService.ValidateContributionAsync(contribution.Id);
                    _logger.LogInformation("Successfully validated contribution {ContributionId}", contribution.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating contribution {ContributionId}", contribution.Id);
                }

            var validatedContributions =
                await _contributionService.GetContributionsByStatusAsync(ContributionStatus.Validated);
            foreach (var contribution in validatedContributions)
                try
                {
                    await _contributionService.ProcessContributionAsync(contribution.Id);
                    _logger.LogInformation("Successfully processed contribution {ContributionId}", contribution.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing contribution {ContributionId}", contribution.Id);
                }

            _logger.LogInformation("Finished processing contributions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in contribution processing job");
            throw;
        }
    }
}