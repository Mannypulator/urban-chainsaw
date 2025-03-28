namespace EPS.Domain.Exceptions;

public class InvalidContributionException : DomainException
{
    public InvalidContributionException(string message)
        : base(message)
    {
    }

    public static InvalidContributionException DuplicateMonthlyContribution(DateTime contributionDate)
    {
        return new InvalidContributionException(
            $"Monthly contribution for {contributionDate:MMMM yyyy} already exists.");
    }

    public static InvalidContributionException InvalidAmount()
    {
        return new InvalidContributionException("Contribution amount must be greater than zero.");
    }

    public static InvalidContributionException FutureContributionDate()
    {
        return new InvalidContributionException("Contribution date cannot be in the future.");
    }
}