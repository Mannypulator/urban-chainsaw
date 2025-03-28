using EPS.Domain.Entities;
using EPS.Domain.Repositories;
using EPS.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EPS.Persistence.Repositories;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(EPSDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetMemberWithContributionsAsync(Guid memberId)
    {
        return await _dbSet
            .Include(m => m.Contributions)
            .Include(m => m.Employer)
            .FirstOrDefaultAsync(m => m.Id == memberId);
    }

    public async Task<IReadOnlyList<Member>> GetMembersByEmployerAsync(Guid employerId)
    {
        return await _dbSet
            .Include(m => m.Contributions)
            .Where(m => m.EmployerId == employerId)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeMemberId = null)
    {
        return !await _dbSet.AnyAsync(m =>
            m.Email == email &&
            (!excludeMemberId.HasValue || m.Id != excludeMemberId.Value));
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, Guid? excludeMemberId = null)
    {
        return !await _dbSet.AnyAsync(m =>
            m.PhoneNumber == phone &&
            (!excludeMemberId.HasValue || m.Id != excludeMemberId.Value));
    }

    public async Task<decimal> GetTotalContributionsAsync(Guid memberId)
    {
        return await _context.Contributions
            .Where(c => c.MemberId == memberId && !c.IsDeleted)
            .SumAsync(c => c.Amount);
    }

    public async Task<int> GetContributionMonthsCountAsync(Guid memberId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.MemberId == memberId && !c.IsDeleted)
            .Select(c => new { c.ContributionDate.Year, c.ContributionDate.Month })
            .Distinct()
            .CountAsync();

        return contributions;
    }

    public async Task<IReadOnlyList<Member>> GetActiveEmployerMembersAsync(Guid employerId)
    {
        return await _dbSet
            .Include(m => m.Contributions)
            .Where(m => m.EmployerId == employerId && !m.IsDeleted)
            .ToListAsync();
    }
}