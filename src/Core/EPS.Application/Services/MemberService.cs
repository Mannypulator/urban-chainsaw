using AutoMapper;
using EPS.Application.DTOs;
using EPS.Domain.Entities;
using EPS.Domain.Exceptions;
using EPS.Domain.Repositories;

namespace EPS.Application.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MemberDto> CreateMemberAsync(CreateMemberDto createMemberDto)
    {
        // Validate age
        var member = _mapper.Map<Member>(createMemberDto);
        var age = member.CalculateAge();
        if (age < 18 || age > 70)
        {
            throw InvalidMemberException.InvalidAge(age);
        }

        // Validate employer exists and is active
        var employer = await _unitOfWork.Employers.GetByIdAsync(createMemberDto.EmployerId);
        if (employer == null || !employer.IsActive)
        {
            throw InvalidMemberException.InvalidEmployer();
        }

        // Validate unique email and phone
        if (!await _unitOfWork.Members.IsEmailUniqueAsync(createMemberDto.Email))
        {
            throw InvalidMemberException.DuplicateEmail(createMemberDto.Email);
        }

        if (!await _unitOfWork.Members.IsPhoneUniqueAsync(createMemberDto.PhoneNumber))
        {
            throw InvalidMemberException.DuplicatePhone(createMemberDto.PhoneNumber);
        }

        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return await GetMemberByIdAsync(member.Id);
    }

    public async Task<MemberDto> UpdateMemberAsync(Guid id, UpdateMemberDto updateMemberDto)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null)
        {
            throw new DomainException($"Member with ID {id} not found.");
        }

        // Validate age if DateOfBirth is provided
        if (updateMemberDto.DateOfBirth.HasValue)
        {
            var age = (DateTime.UtcNow - updateMemberDto.DateOfBirth.Value).Days / 365;
            if (age < 18 || age > 70)
            {
                throw InvalidMemberException.InvalidAge(age);
            }
        }

        // Validate employer exists and is active if EmployerId is provided
        if (updateMemberDto.EmployerId.HasValue)
        {
            var employer = await _unitOfWork.Employers.GetByIdAsync(updateMemberDto.EmployerId.Value);
            if (employer == null || !employer.IsActive)
            {
                throw InvalidMemberException.InvalidEmployer();
            }
        }

        // Validate unique email if provided
        if (!string.IsNullOrWhiteSpace(updateMemberDto.Email) &&
            !await _unitOfWork.Members.IsEmailUniqueAsync(updateMemberDto.Email, id))
        {
            throw InvalidMemberException.DuplicateEmail(updateMemberDto.Email);
        }

        // Validate unique phone if provided
        if (!string.IsNullOrWhiteSpace(updateMemberDto.Phone) &&
            !await _unitOfWork.Members.IsPhoneUniqueAsync(updateMemberDto.Phone, id))
        {
            throw InvalidMemberException.DuplicatePhone(updateMemberDto.Phone);
        }

        _mapper.Map(updateMemberDto, member);
        await _unitOfWork.Members.UpdateAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return await GetMemberByIdAsync(member.Id);
    }

    public async Task<MemberDto> GetMemberByIdAsync(Guid id)
    {
        var member = await _unitOfWork.Members.GetMemberWithContributionsAsync(id);
        if (member == null)
        {
            throw new DomainException($"Member with ID {id} not found.");
        }

        var memberDto = _mapper.Map<MemberDto>(member);
        memberDto.TotalContributions = await _unitOfWork.Members.GetTotalContributionsAsync(id);
        return memberDto;
    }

    public async Task<IReadOnlyList<MemberDto>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        return _mapper.Map<IReadOnlyList<MemberDto>>(members);
    }

    public async Task<IReadOnlyList<MemberDto>> GetMembersByEmployerAsync(Guid employerId)
    {
        var members = await _unitOfWork.Members.GetMembersByEmployerAsync(employerId);
        return _mapper.Map<IReadOnlyList<MemberDto>>(members);
    }

    public async Task DeleteMemberAsync(Guid id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null)
        {
            throw new DomainException($"Member with ID {id} not found.");
        }

        await _unitOfWork.Members.DeleteAsync(member);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalContributionsAsync(Guid memberId)
    {
        if (!await _unitOfWork.Members.ExistsAsync(memberId))
        {
            throw new DomainException($"Member with ID {memberId} not found.");
        }

        return await _unitOfWork.Members.GetTotalContributionsAsync(memberId);
    }

    public async Task<bool> IsMemberEligibleForBenefitsAsync(Guid memberId)
    {
        var member = await _unitOfWork.Members.GetMemberWithContributionsAsync(memberId);
        if (member == null)
        {
            throw new DomainException($"Member with ID {memberId} not found.");
        }

        // Check contribution months
        var contributionMonths = await _unitOfWork.Members.GetContributionMonthsCountAsync(memberId);
        if (contributionMonths < 6) // Minimum 6 months of contributions required
        {
            return false;
        }

        // Check total contributions
        var totalContributions = await _unitOfWork.Members.GetTotalContributionsAsync(memberId);
        return totalContributions >= 50000; // Minimum 50,000 in total contributions required
    }

    public async Task UpdateBenefitEligibilityAsync(Guid memberId)
    {
        var member = await _unitOfWork.Members.GetMemberWithContributionsAsync(memberId);
        if (member == null)
        {
            throw new DomainException($"Member with ID {memberId} not found.");
        }

        var isEligible = await IsMemberEligibleForBenefitsAsync(memberId);
        if (isEligible != member.IsEligibleForBenefits)
        {
            member.IsEligibleForBenefits = isEligible;
            member.BenefitsEligibilityDate = isEligible ? DateTime.UtcNow : null;

            var history = new BenefitEligibilityHistory
            {
                MemberId = memberId,
                IsEligible = isEligible,
                EvaluationDate = DateTime.UtcNow,
                TotalContributions = await _unitOfWork.Members.GetTotalContributionsAsync(memberId),
                ContributionMonths = await _unitOfWork.Members.GetContributionMonthsCountAsync(memberId),
                Reason = isEligible ? "Met eligibility criteria" : "Does not meet eligibility criteria"
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Members.UpdateAsync(member);
                await _unitOfWork.BenefitEligibilityHistories.AddAsync(history);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}