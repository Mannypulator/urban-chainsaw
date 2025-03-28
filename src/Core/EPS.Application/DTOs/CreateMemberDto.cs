namespace EPS.Application.DTOs;

public class CreateMemberDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }

    public string PhoneNumber { get; set; }
    public Guid EmployerId { get; set; }
}