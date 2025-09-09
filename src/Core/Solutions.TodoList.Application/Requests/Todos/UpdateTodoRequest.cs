using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.Application.Requests.Todos;

public record UpdateTodoRequest(TodoTitle Title, TodoDescription Description);
