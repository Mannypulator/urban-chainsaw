using AutoMapper;
using EPS.Application.DTOs;
using EPS.Application.Mappings;
using EPS.Domain.Entities;
using EPS.Domain.Exceptions;
using EPS.Domain.Repositories;

namespace EPS.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MemberDto> CreateMemberAsync(CreateMemberDto createMemberDto)
    {
       
        var member = createMemberDto.MapToEntity();

      
        var employer = await _unitOfWork.Employers.GetByIdAsync(createMemberDto.EmployerId);

        if (employer is not { IsActive: true }) throw new MemberAssociationBadRequestException("Member must be associated with an active employer");

       
        if (!await _unitOfWork.Members.IsEmailUniqueAsync(createMemberDto.Email))
            throw new MemberDuplicateException($"Phone number {createMemberDto.Email} is already registered.");

        if (!await _unitOfWork.Members.IsPhoneUniqueAsync(createMemberDto.PhoneNumber))
            throw new MemberDuplicateException($"Phone number {createMemberDto.PhoneNumber} is already registered.");

        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return await GetMemberByIdAsync(member.Id);
    }

    public async Task<MemberDto> UpdateMemberAsync(Guid id, UpdateMemberDto updateMemberDto)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null) throw new MemberNotFoundException($"Member with ID {id} not found.");

        
        if (updateMemberDto.EmployerId.HasValue)
        {
            var employer = await _unitOfWork.Employers.GetByIdAsync(updateMemberDto.EmployerId.Value);
            if (employer is not { IsActive: true }) throw new MemberAssociationBadRequestException("Member must be associated with an active employer");;
        }

        
        if (!string.IsNullOrWhiteSpace(updateMemberDto.Email) &&
            !await _unitOfWork.Members.IsEmailUniqueAsync(updateMemberDto.Email, id))
            throw new MemberDuplicateException($"Phone number {updateMemberDto.Email} is already registered.");

        
        if (!string.IsNullOrWhiteSpace(updateMemberDto.Phone) &&
            !await _unitOfWork.Members.IsPhoneUniqueAsync(updateMemberDto.Phone, id))
            throw new MemberDuplicateException($"Phone number {updateMemberDto.Phone} is already registered.");

        _mapper.Map(updateMemberDto, member);
        await _unitOfWork.Members.UpdateAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return await GetMemberByIdAsync(member.Id);
    }

    public async Task<MemberDto> GetMemberByIdAsync(Guid id)
    {
        var member = await _unitOfWork.Members.GetMemberWithContributionsAsync(id);
        if (member == null) throw new MemberNotFoundException($"Member with ID {id} not found.");

        var memberDto = member.MapToDto();
        memberDto.TotalContributions = await _unitOfWork.Members.GetTotalContributionsAsync(id);
        return memberDto;
    }

    public async Task<IReadOnlyList<MemberDto>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        return members.MapToDtoList();
    }

    public async Task<IReadOnlyList<MemberDto>> GetMembersByEmployerAsync(Guid employerId)
    {
        var members = await _unitOfWork.Members.GetMembersByEmployerAsync(employerId);
        return members.MapToDtoList();
    }

    public async Task DeleteMemberAsync(Guid id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null) throw new MemberNotFoundException($"Member with ID {id} not found.");

        await _unitOfWork.Members.DeleteAsync(member);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalContributionsAsync(Guid memberId)
    {
        if (!await _unitOfWork.Members.ExistsAsync(memberId))
            throw new MemberNotFoundException($"Member with ID {memberId} not found.");

        return await _unitOfWork.Members.GetTotalContributionsAsync(memberId);
    }

    public async Task<bool> IsMemberEligibleForBenefitsAsync(Guid memberId)
    {
        var member = await _unitOfWork.Members.GetMemberWithContributionsAsync(memberId);
        if (member == null) throw new MemberNotFoundException($"Member with ID {memberId} not found.");

       
        var contributionMonths = await _unitOfWork.Members.GetContributionMonthsCountAsync(memberId);
        if (contributionMonths < 6) // Minimum 6 months of contributions required
            return false;
        
        var totalContributions = await _unitOfWork.Members.GetTotalContributionsAsync(memberId);
        return totalContributions >= 50000;
    }

    public async Task UpdateBenefitEligibilityAsync(Guid memberId)
    {
        var member = await _unitOfWork.Members.GetMemberWithContributionsAsync(memberId);
        if (member == null) throw new MemberNotFoundException($"Member with ID {memberId} not found.");

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