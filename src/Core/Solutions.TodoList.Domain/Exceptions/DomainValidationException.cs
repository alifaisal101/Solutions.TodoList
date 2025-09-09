namespace Solutions.TodoList.Domain.Exceptions;

public sealed class DomainValidationException(string message) : Exception(message);
