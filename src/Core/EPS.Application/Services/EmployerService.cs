using AutoMapper;
using EPS.Application.DTOs;
using EPS.Application.Mappings;
using EPS.Domain.Exceptions;
using EPS.Domain.Repositories;

namespace EPS.Application.Services;

public class EmployerService : IEmployerService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public EmployerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EmployerDto> CreateEmployerAsync(CreateEmployerDto createEmployerDto)
    {
     
        if (await _unitOfWork.Employers.ExistsByRegistrationNumberAsync(createEmployerDto.RegistrationNumber))
            throw InvalidEmployerException.DuplicateRegistrationNumber(createEmployerDto.RegistrationNumber);

        var employer = createEmployerDto.MapToEntity();
        employer.IsActive = true;
        employer.RegistrationDate = DateTime.UtcNow;

        await _unitOfWork.Employers.AddAsync(employer);
        await _unitOfWork.SaveChangesAsync();

        return await GetEmployerByIdAsync(employer.Id);
    }

    public async Task<EmployerDto> GetEmployerByIdAsync(Guid id)
    {
        var employer = await _unitOfWork.Employers.GetByIdAsync(id);
        if (employer == null) throw new EmployeeNotFoundException($"Employer with ID {id} not found.");

        return employer.MapToDto();
    }

    public async Task<IReadOnlyList<EmployerDto>> GetAllEmployersAsync()
    {
        var employers = await _unitOfWork.Employers.GetAllAsync();
        return employers.MapToDtoList();
    }

    public async Task<IReadOnlyList<EmployerDto>> GetActiveEmployersAsync()
    {
        var employers = await _unitOfWork.Employers.GetActiveEmployersAsync();
        return employers.MapToDtoList();
    }

    public async Task UpdateEmployerAsync(Guid id, UpdateEmployerDto updateEmployerDto)
    {
        var employer = await _unitOfWork.Employers.GetByIdAsync(id);
        if (employer == null) throw new EmployeeNotFoundException($"Employer with ID {id} not found.");

       
        if (updateEmployerDto.RegistrationNumber != employer.RegistrationNumber &&
            !string.IsNullOrWhiteSpace(updateEmployerDto.RegistrationNumber) &&
            await _unitOfWork.Employers.ExistsByRegistrationNumberAsync(updateEmployerDto.RegistrationNumber))
            throw InvalidEmployerException.DuplicateRegistrationNumber(updateEmployerDto.RegistrationNumber);

 
        employer.CompanyName = updateEmployerDto.Name;
        employer.RegistrationNumber = updateEmployerDto.RegistrationNumber;
        employer.Address = updateEmployerDto.Address;
        employer.ContactPerson = updateEmployerDto.ContactPerson;
        employer.ContactEmail = updateEmployerDto.Email;
        employer.ContactPhone = updateEmployerDto.Phone;

        if (updateEmployerDto.IsActive.HasValue) employer.IsActive = updateEmployerDto.IsActive.Value;

        await _unitOfWork.Employers.UpdateAsync(employer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateEmployerAsync(Guid id)
    {
        var employer = await _unitOfWork.Employers.GetByIdAsync(id);
        if (employer == null) throw new EmployeeNotFoundException($"Employer with ID {id} not found.");

        // Check if employer has active members
        var activeMembers = await _unitOfWork.Members.GetActiveEmployerMembersAsync(id);
        if (activeMembers.Any()) throw new EmployeeBadRequestException("Cannot deactivate employer with active members.");

        employer.IsActive = false;
        await _unitOfWork.Employers.UpdateAsync(employer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<MemberDto>> GetEmployerMembersAsync(Guid employerId)
    {
        var employer = await _unitOfWork.Employers.GetByIdAsync(employerId);
        if (employer == null) throw new EmployeeNotFoundException($"Employer with ID {employerId} not found.");

        var members = await _unitOfWork.Members.GetMembersByEmployerAsync(employerId);
        return members.MapToDtoList();
    }

    public async Task<decimal> GetTotalContributionsAsync(Guid employerId, DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var employer = await _unitOfWork.Employers.GetByIdAsync(employerId);
        if (employer == null) throw new EmployeeNotFoundException($"Employer with ID {employerId} not found.");

        return await _unitOfWork.Contributions.GetTotalEmployerContributionsAsync(employerId, startDate, endDate);
    }

    public async Task<bool> ValidateEmployerAsync(Guid employerId)
    {
        var employer = await _unitOfWork.Employers.GetByIdAsync(employerId);
        return employer != null && employer.IsActive;
    }
}