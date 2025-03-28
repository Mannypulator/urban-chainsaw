using EPS.Domain.Entities;
using EPS.Domain.Repositories;
using EPS.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EPS.Persistence.Repositories;

public class EmployerRepository : GenericRepository<Employer>, IEmployerRepository
{
    public EmployerRepository(EPSDbContext context) : base(context)
    {
    }

    public async Task<Employer?> GetEmployerWithMembersAsync(Guid employerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.Members)
            .FirstOrDefaultAsync(e => e.Id == employerId);
    }

    public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, Guid? excludeEmployerId = null)
    {
        return !await _dbSet.AnyAsync(e =>
            e.RegistrationNumber == registrationNumber &&
            (!excludeEmployerId.HasValue || e.Id != excludeEmployerId.Value));
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeEmployerId = null)
    {
        return !await _dbSet.AnyAsync(e =>
            e.ContactEmail == email &&
            (!excludeEmployerId.HasValue || e.Id != excludeEmployerId.Value));
    }

    public async Task<IReadOnlyList<Employer>> GetActiveEmployersAsync()
    {
        return await _dbSet
            .Where(e => e.IsActive)
            .ToListAsync();
    }

    public async Task<int> GetTotalMembersCountAsync(Guid employerId)
    {
        return await _context.Members
            .CountAsync(m => m.EmployerId == employerId && !m.IsDeleted);
    }

    public async Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber)
    {
        return await _dbSet.AnyAsync(e => e.RegistrationNumber == registrationNumber);
    }
}