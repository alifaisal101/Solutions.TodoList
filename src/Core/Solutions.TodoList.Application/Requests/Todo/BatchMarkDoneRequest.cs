namespace Solutions.TodoList.Application.Requests.Todo;

public record BatchMarkDoneRequest(Guid[] Ids, bool Done);
