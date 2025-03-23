using EPS.Application.Services;
using Microsoft.Extensions.Logging;

namespace EPS.Infrastructure.BackgroundJobs.Jobs;

public class BenefitEligibilityJob
{
    private readonly IMemberService _memberService;
    private readonly ILogger<BenefitEligibilityJob> _logger;

    public BenefitEligibilityJob(
        IMemberService memberService,
        ILogger<BenefitEligibilityJob> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    public async Task UpdateBenefitEligibilityForAllMembersAsync()
    {
        try
        {
            _logger.LogInformation("Starting benefit eligibility update for all members...");

            var members = await _memberService.GetAllMembersAsync();
            foreach (var member in members)
            {
                try
                {
                    await _memberService.UpdateBenefitEligibilityAsync(member.Id);
                    _logger.LogInformation("Successfully updated benefit eligibility for member {MemberId}", member.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating benefit eligibility for member {MemberId}", member.Id);
                }
            }

            _logger.LogInformation("Finished updating benefit eligibility for all members");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in benefit eligibility update job");
            throw;
        }
    }
}