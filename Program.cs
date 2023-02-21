

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDbContext<TodoDb>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/todos",async (TodoDb db) => await db.todoItems.ToListAsync());

app.MapGet("/todos/{id}", async (int id, TodoDb db) => 
    await db.todoItems.FirstOrDefaultAsync(h => h.Id == id) is Todo todo
    ? Results.Ok(todo)
    : Results.NotFound());

app.MapPost("todos", async ([FromBody] Todo todo, TodoDb db) => 
    {
        db.todoItems.Add(todo);
        await db.SaveChangesAsync();
        return Results.Created($"/hotels/{todo.Id}", todo);
    });

app.MapPut("/todos", async ([FromBody] Todo todo, TodoDb db) => {
    var todoFromDb = await db.todoItems.FindAsync(new object[] {todo.Id});
    if (todoFromDb == null) return Results.NotFound();
    todoFromDb.Name = todo.Name;
    todoFromDb.IsComplete = todo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("todos/{id}", async (int id, TodoDb db) => {
    var todoFromDb = await db.todoItems.FindAsync(new object[] {id});
    if (todoFromDb == null) return Results.NotFound();
    db.todoItems.Remove(todoFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();
