using System.ComponentModel.DataAnnotations;
using EPS.Domain.Common;

namespace EPS.Domain.Entities;

public class Employer : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContactPerson { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; }

    // Navigation properties
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public Employer()
    {
        Members = new HashSet<Member>();
        IsActive = true;
    }
}