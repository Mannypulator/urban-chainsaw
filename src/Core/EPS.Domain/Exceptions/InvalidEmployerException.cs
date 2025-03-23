namespace EPS.Domain.Exceptions;

public class InvalidEmployerException : DomainException
{
    private InvalidEmployerException(string message) : base(message) { }

    public static InvalidEmployerException InvalidName() =>
        new("Employer name cannot be empty.");

    public static InvalidEmployerException InvalidRegistrationNumber() =>
        new("Employer registration number cannot be empty.");

    public static InvalidEmployerException DuplicateRegistrationNumber(string number) =>
        new($"An employer with registration number {number} already exists.");
} 