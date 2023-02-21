public interface ITodoRepository : IDisposable
{
    Task<List<Todo>> GetTodosAsync();
    Task<List<Todo>> GetTodosAsync(string name);
    Task<List<Todo>> GetTodosAsync(bool iscomplete);   
    Task<Todo?> GetTodoAsync(int todoId);
    Task InsertTodoAsync(Todo todo);
    Task UpdateTodoAsync(Todo todo);
    Task DeleteTodoAsync(int todoId);
    Task SaveAsync();

}