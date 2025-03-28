namespace EPS.Domain.Exceptions;

public class RequestObjectBadRequestException(string message) : BadRequestException(message)
{
}