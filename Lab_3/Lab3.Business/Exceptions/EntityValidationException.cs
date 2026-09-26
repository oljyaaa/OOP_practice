namespace Lab3.Business.Exceptions;

public sealed class EntityValidationException(string message) : Exception(message);
