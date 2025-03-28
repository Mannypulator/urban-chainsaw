namespace EPS.Domain.Exceptions;

public class MemberNotFoundException(string message) : NotFoundException(message);