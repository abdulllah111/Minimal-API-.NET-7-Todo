public interface ITodoRepository : IDisposable
{
    Task<List<Todo>> GetTodosAsync();
    Task<Todo?> GetTodoAsync(int todoId);
    Task InsertTodoAsync(Todo todo);
    Task UpdateTodoAsync(Todo todo);
    Task DeleteTodoAsync(int todoId);
    Task SaveAsync();

}