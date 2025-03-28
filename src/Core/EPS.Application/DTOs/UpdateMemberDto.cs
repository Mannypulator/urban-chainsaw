namespace EPS.Application.DTOs;

public class UpdateMemberDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public Guid? EmployerId { get; set; }
}