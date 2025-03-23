using EPS.Domain.Entities;

namespace EPS.Domain.Repositories;

public interface IBenefitEligibilityHistoryRepository : IRepository<BenefitEligibilityHistory>
{
    Task<IReadOnlyList<BenefitEligibilityHistory>> GetMemberHistoryAsync(Guid memberId);
}