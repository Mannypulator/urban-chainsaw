using EPS.Application.DTOs;
using EPS.Domain.Entities;

namespace EPS.Application.Mappings;

public static class MemberMapping
{
    public static Member MapToEntity(this CreateMemberDto dto)
    {
        return new Member
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            EmployerId = dto.EmployerId,
            CreatedAt = DateTime.Now
        };
    }

    public static MemberDto MapToDto(this Member entity)
    {
        return new MemberDto
        {
            Id = entity.Id,
            Age = entity.CalculateAge(),
            BenefitsEligibilityDate = entity.BenefitsEligibilityDate,
            DateOfBirth = entity.DateOfBirth,
            Email = entity.Email,
            EmployerId = entity.EmployerId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PhoneNumber = entity.PhoneNumber,
            TotalContributions = entity.Contributions.Count,
            IsEligibleForBenefits = entity.IsEligibleForBenefits
        };
    }


    public static IReadOnlyList<MemberDto> MapToDtoList(this IReadOnlyList<Member> enities)
    {
        return enities.Select(entity => entity.MapToDto()).ToList();
    }
}