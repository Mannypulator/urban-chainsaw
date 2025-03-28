using AutoMapper;
using EPS.Application.DTOs;
using EPS.Application.Mappings;
using EPS.Domain.Enums;
using EPS.Domain.Exceptions;
using EPS.Domain.Repositories;

namespace EPS.Application.Services;

public class ContributionService : IContributionService
{
    private readonly IMapper _mapper;
    private readonly IMemberService _memberService;
    private readonly IUnitOfWork _unitOfWork;

    public ContributionService(IUnitOfWork unitOfWork, IMapper mapper, IMemberService memberService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memberService = memberService;
    }

    public async Task<ContributionDto> CreateContributionAsync(CreateContributionDto createContributionDto)
    {
        // Validate member exists
        var member = await _unitOfWork.Members.GetByIdAsync(createContributionDto.MemberId)
                     ?? throw new DomainException($"Member with ID {createContributionDto.MemberId} not found.");


        var contribution = createContributionDto.MapToEntity();
        await _unitOfWork.Contributions.AddAsync(contribution);
        await _unitOfWork.SaveChangesAsync();

        return await GetContributionByIdAsync(contribution.Id);
    }

    public async Task<ContributionDto> GetContributionByIdAsync(Guid id)
    {
        var contribution = await _unitOfWork.Contributions.GetByIdAsync(id);
        if (contribution == null) throw new DomainException($"Contribution with ID {id} not found.");

        return contribution.MapToDto();
    }

    public async Task<IReadOnlyList<ContributionDto>> GetMemberContributionsAsync(Guid memberId)
    {
        var contributions = await _unitOfWork.Contributions.GetMemberContributionsAsync(memberId);
        return contributions.MapDtoList();
    }

    public async Task<IReadOnlyList<ContributionDto>> GetContributionsByStatusAsync(ContributionStatus status)
    {
        var contributions = await _unitOfWork.Contributions.GetContributionsByStatusAsync(status);
        return contributions.MapDtoList();
        ;
    }

    public async Task ValidateContributionAsync(Guid contributionId)
    {
        var contribution = await _unitOfWork.Contributions.GetByIdAsync(contributionId);
        if (contribution == null) throw new DomainException($"Contribution with ID {contributionId} not found.");

        if (contribution.Status != ContributionStatus.Pending)
            throw new DomainException("Only pending contributions can be validated.");

        // Perform validation logic
        var isValid = true;
        var validationMessage = "";

        try
        {
            // Add validation rules here
            if (contribution.Amount <= 0)
            {
                isValid = false;
                validationMessage = "Invalid contribution amount.";
            }

            if (contribution.ContributionDate > DateTime.UtcNow)
            {
                isValid = false;
                validationMessage = "Contribution date cannot be in the future.";
            }

            contribution.Status = isValid ? ContributionStatus.Validated : ContributionStatus.Failed;
            contribution.ValidationMessage = validationMessage;

            await _unitOfWork.Contributions.UpdateAsync(contribution);
            await _unitOfWork.SaveChangesAsync();

            // If contribution is valid, update member's benefit eligibility
            if (isValid) await _memberService.UpdateBenefitEligibilityAsync(contribution.MemberId);
        }
        catch (Exception ex)
        {
            contribution.Status = ContributionStatus.Failed;
            contribution.ValidationMessage = $"Validation error: {ex.Message}";
            await _unitOfWork.Contributions.UpdateAsync(contribution);
            await _unitOfWork.SaveChangesAsync();
            throw;
        }
    }

    public async Task ProcessContributionAsync(Guid contributionId)
    {
        var contribution = await _unitOfWork.Contributions.GetByIdAsync(contributionId);
        if (contribution == null) throw new DomainException($"Contribution with ID {contributionId} not found.");

        if (contribution.Status != ContributionStatus.Validated)
            throw new DomainException("Only validated contributions can be processed.");

        try
        {
            // Add processing logic here
            contribution.Status = ContributionStatus.Processed;
            await _unitOfWork.Contributions.UpdateAsync(contribution);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            contribution.Status = ContributionStatus.Failed;
            contribution.ValidationMessage = $"Processing error: {ex.Message}";
            await _unitOfWork.Contributions.UpdateAsync(contribution);
            await _unitOfWork.SaveChangesAsync();
            throw;
        }
    }

    public async Task CalculateInterestAsync(Guid contributionId)
    {
        var contribution = await _unitOfWork.Contributions.GetByIdAsync(contributionId);
        if (contribution == null) throw new DomainException($"Contribution with ID {contributionId} not found.");

        if (contribution.Status != ContributionStatus.Processed)
            throw new DomainException("Only processed contributions can earn interest.");

        if (contribution.InterestCalculationDate.HasValue)
            throw new DomainException("Interest has already been calculated for this contribution.");

        // Calculate interest (example: 5% annual interest, prorated by months)
        var monthsHeld = (DateTime.UtcNow - contribution.ContributionDate).Days / 30.0;
        var annualRate = 0.05m; // 5% annual interest
        var interest = contribution.Amount * (annualRate / 12) * (decimal)monthsHeld;

        contribution.InterestEarned = interest;
        contribution.InterestCalculationDate = DateTime.UtcNow;

        await _unitOfWork.Contributions.UpdateAsync(contribution);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalContributionsForPeriodAsync(Guid memberId, DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.Contributions.GetTotalContributionsForPeriodAsync(memberId, startDate, endDate);
    }

    public async Task<bool> ValidateMonthlyContributionAsync(Guid memberId, DateTime contributionDate)
    {
        return await _unitOfWork.Contributions.HasMonthlyContributionAsync(memberId, contributionDate);
    }
}