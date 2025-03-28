using EPS.Application.DTOs;
using FluentValidation;

namespace EPS.Application.Validators;

public class UpdateMemberDtoValidator : AbstractValidator<UpdateMemberDto>
{
    public UpdateMemberDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.NationalId)
            .MaximumLength(50).WithMessage("National ID cannot exceed 50 characters.");

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidAge).When(x => x.DateOfBirth.HasValue)
            .WithMessage("Member must be between 18 and 70 years old.");

        RuleFor(x => x.Address)
            .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9]{7,20}$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Invalid phone number format.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");
    }

    private bool BeValidAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue) return false;

        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Value.Year;
        if (dateOfBirth.Value.Date > today.AddYears(-age)) age--;

        return age >= 18 && age <= 70;
    }
}