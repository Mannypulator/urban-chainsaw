using EPS.Application.DTOs;
using EPS.Domain.Enums;

namespace EPS.Application.Services;

public interface IContributionService
{
    Task<ContributionDto> CreateContributionAsync(CreateContributionDto createContributionDto);
    Task<ContributionDto> GetContributionByIdAsync(Guid id);
    Task<IReadOnlyList<ContributionDto>> GetMemberContributionsAsync(Guid memberId);
    Task<IReadOnlyList<ContributionDto>> GetContributionsByStatusAsync(ContributionStatus status);
    Task ValidateContributionAsync(Guid contributionId);
    Task ProcessContributionAsync(Guid contributionId);
    Task CalculateInterestAsync(Guid contributionId);
    Task<decimal> GetTotalContributionsForPeriodAsync(Guid memberId, DateTime startDate, DateTime endDate);
    Task<bool> ValidateMonthlyContributionAsync(Guid memberId, DateTime contributionDate);
}