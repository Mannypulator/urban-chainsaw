using System.ComponentModel.DataAnnotations;
using EPS.Domain.Enums;

namespace EPS.Application.DTOs;

public class CreateContributionDto
{
    [Required]
    public Guid MemberId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ContributionDate { get; set; }

    [Required]
    public ContributionType Type { get; set; }

    public string TransactionReference { get; set; }
}