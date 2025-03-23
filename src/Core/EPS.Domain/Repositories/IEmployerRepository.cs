using EPS.Domain.Entities;

namespace EPS.Domain.Repositories;

public interface IEmployerRepository : IGenericRepository<Employer>
{
    Task<Employer?> GetEmployerWithMembersAsync(Guid employerId);
    Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, Guid? excludeEmployerId = null);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeEmployerId = null);
    Task<IReadOnlyList<Employer>> GetActiveEmployersAsync();
    Task<int> GetTotalMembersCountAsync(Guid employerId);
    Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber);
}