using EPS.Application.DTOs;
using EPS.Domain.Entities;

namespace EPS.Application.Mappings;

public static class ContributionMapping
{
    public static Contribution MapToEntity(this CreateContributionDto dto)
    {
        return new Contribution
        {
            MemberId = dto.MemberId,
            Amount = dto.Amount,
            ContributionDate = dto.ContributionDate,
            Type = dto.Type,
            TransactionReference = dto.TransactionReference
        };
    }

    public static ContributionDto MapToDto(this Contribution entity)
    {
        return new ContributionDto
        {
            Amount = entity.Amount,
            ContributionDate = entity.ContributionDate,
            Id = entity.Id,
            InterestCalculationDate = entity.InterestCalculationDate,
            InterestEarned = entity.InterestEarned,
            MemberId = entity.MemberId,
            TransactionReference = entity.TransactionReference,
            Status = entity.Status,
            Type = entity.Type,
            ValidationMessage = entity.ValidationMessage,
            MemberName = $"{entity.Member.FirstName} {entity.Member.LastName}"
        };
    }

    public static IReadOnlyList<ContributionDto> MapDtoList(this IReadOnlyList<Contribution> entities)
    {
        return entities.Select(c => c.MapToDto()).ToList();
    }
}