using EPS.Domain.Entities;
using EPS.Domain.Enums;

namespace EPS.Domain.Repositories;

public interface IContributionRepository : IGenericRepository<Contribution>
{
    Task<IReadOnlyList<Contribution>> GetMemberContributionsAsync(Guid memberId);
    Task<bool> HasMonthlyContributionAsync(Guid memberId, DateTime contributionDate);
    Task<IReadOnlyList<Contribution>> GetPendingContributionsAsync();
    Task<IReadOnlyList<Contribution>> GetContributionsByStatusAsync(ContributionStatus status);
    Task<decimal> GetTotalContributionsForPeriodAsync(Guid memberId, DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Contribution>> GetContributionsForInterestCalculationAsync(DateTime cutoffDate);
    Task<decimal> GetTotalEmployerContributionsAsync(Guid employerId, DateTime? startDate = null, DateTime? endDate = null);
}