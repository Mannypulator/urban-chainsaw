namespace EPS.Application.DTOs;

public class UpdateEmployerDto
{
    public string? Name { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? IsActive { get; set; }
}