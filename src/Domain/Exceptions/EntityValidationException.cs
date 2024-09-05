namespace Domain.Exceptions;

public class EntityValidationException(string? message) : Exception(message)
{
}