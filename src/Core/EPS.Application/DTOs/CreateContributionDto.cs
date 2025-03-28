using EPS.Domain.Enums;

namespace EPS.Application.DTOs;

public class CreateContributionDto
{
    public Guid MemberId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public ContributionType Type { get; set; }
    public string TransactionReference { get; set; }
}