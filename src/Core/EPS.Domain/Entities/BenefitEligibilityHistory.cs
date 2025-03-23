using System.ComponentModel.DataAnnotations;
using EPS.Domain.Common;

namespace EPS.Domain.Entities;

public class BenefitEligibilityHistory : BaseEntity
{
    [Required]
    public Guid MemberId { get; set; }
    public virtual Member Member { get; set; }

    [Required]
    public bool IsEligible { get; set; }

    [Required]
    public DateTime EvaluationDate { get; set; }

    public string Reason { get; set; }

    public decimal TotalContributions { get; set; }
    public int ContributionMonths { get; set; }
}