using EPS.Domain.Entities;
using EPS.Domain.Repositories;
using EPS.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EPS.Persistence.Repositories;

public class BenefitEligibilityHistoryRepository : GenericRepository<BenefitEligibilityHistory>,
    IBenefitEligibilityHistoryRepository
{
    public BenefitEligibilityHistoryRepository(EPSDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<BenefitEligibilityHistory>> GetMemberHistoryAsync(Guid memberId)
    {
        return await _dbSet
            .Include(h => h.Member)
            .Where(h => h.MemberId == memberId)
            .OrderByDescending(h => h.EvaluationDate)
            .ToListAsync();
    }
}