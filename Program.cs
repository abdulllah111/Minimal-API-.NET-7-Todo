

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

app.MapGet("/todos",async (ITodoRepository repository) => 
    Results.Ok(await repository.GetTodosAsync()));

app.MapGet("/todos/{id}", async (int id, ITodoRepository repository) => 
    await repository.GetTodoAsync(id) is Todo todo
    ? Results.Ok(todo)
    : Results.NotFound());

app.MapPost("todos", async ([FromBody] Todo todo, ITodoRepository repository) => 
    {
        await repository.InsertTodoAsync(todo);
        await repository.SaveAsync();
        return Results.Created($"/hotels/{todo.Id}", todo);
    });

app.MapPut("/todos", async ([FromBody] Todo todo, ITodoRepository repository) => {
    await repository.UpdateTodoAsync(todo);
    await repository.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("todos/{id}", async (int id, ITodoRepository repository) => {
    await repository.DeleteTodoAsync(id);
    await repository.SaveAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();
