using System.ComponentModel.DataAnnotations;
using EPS.Domain.Common;

namespace EPS.Domain.Entities;

public class Member : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    public Guid EmployerId { get; set; }
    public virtual Employer Employer { get; set; }

    public virtual ICollection<Contribution> Contributions { get; set; }

    public bool IsEligibleForBenefits { get; set; }
    public DateTime? BenefitsEligibilityDate { get; set; }

    // Navigation properties
    public virtual ICollection<BenefitEligibilityHistory> EligibilityHistory { get; set; }

    public Member()
    {
        Contributions = new HashSet<Contribution>();
        EligibilityHistory = new HashSet<BenefitEligibilityHistory>();
    }

    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}