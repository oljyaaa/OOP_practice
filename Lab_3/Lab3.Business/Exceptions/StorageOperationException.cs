namespace Lab3.Business.Exceptions;

public sealed class StorageOperationException(string message, Exception innerException) : Exception(message, innerException);
