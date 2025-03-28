using EPS.Domain.Entities;
using EPS.Domain.Enums;
using EPS.Domain.Repositories;
using EPS.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EPS.Persistence.Repositories;

public class ContributionRepository : GenericRepository<Contribution>, IContributionRepository
{
    public ContributionRepository(EPSDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Contribution>> GetMemberContributionsAsync(Guid memberId)
    {
        return await _dbSet
            .Include(c => c.Member)
            .Where(c => c.MemberId == memberId)
            .OrderByDescending(c => c.ContributionDate)
            .ToListAsync();
    }

    public async Task<bool> HasMonthlyContributionAsync(Guid memberId, DateTime contributionDate)
    {
        return await _dbSet.AnyAsync(c =>
            c.MemberId == memberId &&
            c.Type == ContributionType.Monthly &&
            c.ContributionDate.Year == contributionDate.Year &&
            c.ContributionDate.Month == contributionDate.Month);
    }

    public async Task<IReadOnlyList<Contribution>> GetPendingContributionsAsync()
    {
        return await _dbSet
            .Include(c => c.Member)
            .Where(c => c.Status == ContributionStatus.Pending)
            .OrderBy(c => c.ContributionDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Contribution>> GetContributionsByStatusAsync(ContributionStatus status)
    {
        return await _dbSet
            .Include(c => c.Member)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.ContributionDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalContributionsForPeriodAsync(Guid memberId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(c => c.MemberId == memberId &&
                        c.ContributionDate >= startDate &&
                        c.ContributionDate <= endDate)
            .SumAsync(c => c.Amount);
    }

    public async Task<IReadOnlyList<Contribution>> GetContributionsForInterestCalculationAsync(DateTime cutoffDate)
    {
        return await _dbSet
            .Include(c => c.Member)
            .Where(c => c.Status == ContributionStatus.Processed &&
                        c.InterestCalculationDate == null &&
                        c.ContributionDate <= cutoffDate)
            .OrderBy(c => c.ContributionDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalEmployerContributionsAsync(Guid employerId, DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbSet
            .Include(c => c.Member)
            .Where(c => c.Member.EmployerId == employerId);

        if (startDate.HasValue) query = query.Where(c => c.ContributionDate >= startDate.Value);

        if (endDate.HasValue) query = query.Where(c => c.ContributionDate <= endDate.Value);

        return await query.SumAsync(c => c.Amount);
    }
}