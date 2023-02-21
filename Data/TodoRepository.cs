public class TodoRepository : ITodoRepository
{ 
    private readonly TodoDb _context;
    public TodoRepository(TodoDb context){
        _context = context;
    }
    public Task<List<Todo>> GetTodosAsync() => _context.todoItems.ToListAsync();

    public Task<List<Todo>> GetTodosAsync(string name) =>
        _context.todoItems.Where(h => h.Name.Contains(name)).ToListAsync();

    public Task<List<Todo>> GetTodosAsync(bool iscomplete) =>
        _context.todoItems.Where(h => h.IsComplete == iscomplete).ToListAsync();
    public async Task<Todo?> GetTodoAsync(int todoId) => await _context.todoItems.FindAsync(new object[]{todoId});

    public async Task InsertTodoAsync(Todo todo) => await _context.todoItems.AddAsync(todo);

    public async Task UpdateTodoAsync(Todo todo)
    {
        var todoFromDb = await _context.todoItems.FindAsync(new object[]{todo.Id});
        if (todoFromDb == null) return;
        todoFromDb.Name = todo.Name;
        todoFromDb.IsComplete = todo.IsComplete;
    }

    public async Task DeleteTodoAsync(int todoId)
    {
        var todoFromDb = await _context.todoItems.FindAsync(new object[]{todoId});
        if (todoFromDb == null) return;
        _context.todoItems.Remove(todoFromDb);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    private bool _disposed = false;
    protected virtual void Dispose(bool disposing){
        if(!_disposed){
            if(disposing){
                _context.Dispose();
            }
        }
        _disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}