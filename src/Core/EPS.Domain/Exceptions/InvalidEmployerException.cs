namespace EPS.Domain.Exceptions;

public class InvalidEmployerException : DomainException
{
    private InvalidEmployerException(string message) : base(message)
    {
    }

    public static InvalidEmployerException InvalidName()
    {
        return new InvalidEmployerException("Employer name cannot be empty.");
    }

    public static InvalidEmployerException InvalidRegistrationNumber()
    {
        return new InvalidEmployerException("Employer registration number cannot be empty.");
    }

    public static InvalidEmployerException DuplicateRegistrationNumber(string number)
    {
        return new InvalidEmployerException($"An employer with registration number {number} already exists.");
    }
}