using EPS.Application.DTOs;

namespace EPS.Application.Services;

public interface IEmployerService
{
    Task<EmployerDto> CreateEmployerAsync(CreateEmployerDto createEmployerDto);
    Task<EmployerDto> GetEmployerByIdAsync(Guid id);
    Task<IReadOnlyList<EmployerDto>> GetAllEmployersAsync();
    Task<IReadOnlyList<EmployerDto>> GetActiveEmployersAsync();
    Task UpdateEmployerAsync(Guid id, UpdateEmployerDto updateEmployerDto);
    Task DeactivateEmployerAsync(Guid id);
    Task<IReadOnlyList<MemberDto>> GetEmployerMembersAsync(Guid employerId);
    Task<decimal> GetTotalContributionsAsync(Guid employerId, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> ValidateEmployerAsync(Guid employerId);
}