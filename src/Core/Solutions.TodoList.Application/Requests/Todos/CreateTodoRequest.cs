using Solutions.TodoList.Domain.ValueObjects;

namespace Solutions.TodoList.Application.Requests.Todos;

public record CreateTodoRequest(TodoTitle Title, TodoDescription Description);
