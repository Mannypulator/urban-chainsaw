using EPS.Application.DTOs;
using FluentValidation;

namespace EPS.Application.Validators;

public class CreateContributionDtoValidator : AbstractValidator<CreateContributionDto>
{
    public CreateContributionDtoValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.ContributionDate)
            .NotEmpty().WithMessage("Contribution date is required.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Contribution date cannot be in the future.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid contribution type.");

        RuleFor(x => x.TransactionReference)
            .MaximumLength(100).WithMessage("Transaction reference cannot exceed 100 characters.");
    }
}