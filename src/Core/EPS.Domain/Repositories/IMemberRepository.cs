using EPS.Domain.Entities;

namespace EPS.Domain.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    Task<IReadOnlyList<Member>> GetMembersByEmployerAsync(Guid employerId);
    Task<IReadOnlyList<Member>> GetActiveEmployerMembersAsync(Guid employerId);
    Task<Member?> GetMemberWithContributionsAsync(Guid id);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
    Task<bool> IsPhoneUniqueAsync(string phone, Guid? excludeId = null);
    Task<decimal> GetTotalContributionsAsync(Guid memberId);
    Task<int> GetContributionMonthsCountAsync(Guid memberId);
}