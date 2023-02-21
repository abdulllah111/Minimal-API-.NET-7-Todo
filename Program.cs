var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var todoItems = new List<Todo>();

app.MapGet("/todos", () => todoItems);
app.MapGet("/todos/{id}", (int id) => todoItems.FirstOrDefault(h => h.Id == id));
app.MapPost("todos", (Todo todo) => todoItems.Add(todo));
app.MapPut("/todos", (Todo todo) => {
    var index = todoItems.FindIndex(h => h.Id == todo.Id);
    if(index < 0) throw new Exception("Not found ");
    todoItems[index] = todo;
});
app.MapDelete("hotels/{id}", (int id) => {
    var index = todoItems.FindIndex(h => h.Id == id);
    if (index < 0) throw new Exception("Not found");
    todoItems.RemoveAt(index);
});

app.Run();


public class Todo 
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}