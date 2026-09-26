namespace Lab3.Business.Exceptions;

public sealed class EntityNotFoundException(string message) : Exception(message);
