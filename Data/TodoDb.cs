public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options): base(options) {}
    public DbSet<Todo> todoItems => Set<Todo>();
    public DbSet<User> users => Set<User>();

}