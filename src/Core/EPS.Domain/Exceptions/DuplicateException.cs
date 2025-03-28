namespace EPS.Domain.Exceptions;

public abstract class DuplicateException(string message) : Exception(message);