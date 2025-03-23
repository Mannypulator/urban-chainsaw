namespace EPS.Application.DTOs;

public class MemberDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid EmployerId { get; set; }
    public string EmployerName { get; set; }
    public bool IsEligibleForBenefits { get; set; }
    public DateTime? BenefitsEligibilityDate { get; set; }
    public decimal TotalContributions { get; set; }
    public int Age { get; set; }
}