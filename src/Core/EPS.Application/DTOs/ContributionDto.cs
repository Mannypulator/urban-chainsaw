using EPS.Domain.Enums;

namespace EPS.Application.DTOs;

public class ContributionDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public ContributionType Type { get; set; }
    public string TransactionReference { get; set; }
    public ContributionStatus Status { get; set; }
    public string ValidationMessage { get; set; }
    public decimal? InterestEarned { get; set; }
    public DateTime? InterestCalculationDate { get; set; }
}