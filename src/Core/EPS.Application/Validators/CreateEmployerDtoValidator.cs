using EPS.Application.DTOs;
using FluentValidation;

namespace EPS.Application.Validators;

public class CreateEmployerDtoValidator : AbstractValidator<CreateEmployerDto>
{
    public CreateEmployerDtoValidator()
    {
        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .NotNull()
            .WithMessage("Employer registration number must be provided");

        RuleFor(x => x.Address)
            .NotEmpty()
            .NotNull()
            .WithMessage("Address must be provided");

        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Name must be provided");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Valid Email address must be provided")
            .NotEmpty()
            .WithMessage("Email address must be provided");

        RuleFor(x => x.ContactPerson)
            .NotEmpty()
            .NotNull()
            .WithMessage("Contact person must be provided");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .NotNull()
            .WithMessage("Phone number must be provided");
    }
}