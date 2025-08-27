namespace Solutions.TodoList.Application.Requests.Todos;

public record BatchMarkDoneRequest(Guid[] Ids, bool Done);
