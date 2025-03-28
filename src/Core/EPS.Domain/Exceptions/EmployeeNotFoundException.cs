namespace EPS.Domain.Exceptions;

public class EmployeeNotFoundException(string message) : NotFoundException(message);