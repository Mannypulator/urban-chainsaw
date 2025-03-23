using System.ComponentModel.DataAnnotations;
using EPS.Domain.Common;
using EPS.Domain.Enums;

namespace EPS.Domain.Entities;

public class Contribution : BaseEntity
{
    [Required]
    public Guid MemberId { get; set; }
    public virtual Member Member { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Contribution amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ContributionDate { get; set; }

    [Required]
    public ContributionType Type { get; set; }

    public string TransactionReference { get; set; }

    [Required]
    public ContributionStatus Status { get; set; }

    public string ValidationMessage { get; set; }

    public decimal? InterestEarned { get; set; }
    public DateTime? InterestCalculationDate { get; set; }

    public Contribution()
    {
        Status = ContributionStatus.Pending;
    }
}