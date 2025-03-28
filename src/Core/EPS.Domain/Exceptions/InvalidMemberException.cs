namespace EPS.Domain.Exceptions;

public class InvalidMemberException : DomainException
{
    public InvalidMemberException(string message)
        : base(message)
    {
    }

    public static InvalidMemberException InvalidAge(int age)
    {
        return new InvalidMemberException($"Member age must be between 18 and 70 years. Current age: {age}");
    }

    public static InvalidMemberException DuplicateEmail(string email)
    {
        return new InvalidMemberException($"Email address {email} is already registered.");
    }

    public static InvalidMemberException DuplicatePhone(string phone)
    {
        return new InvalidMemberException($"Phone number {phone} is already registered.");
    }

    public static InvalidMemberException InvalidEmployer()
    {
        return new InvalidMemberException("Member must be associated with an active employer.");
    }
}