var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDbContext<TodoDb>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/todos",async (ITodoRepository repository) => 
    Results.Ok(await repository.GetTodosAsync()))
    .Produces<List<Todo>>(StatusCodes.Status200OK)
    .WithName("GetAllTodoItems")
    .WithTags("Getters");

app.MapGet("/todos/{id}", async (int id, ITodoRepository repository) => 
    await repository.GetTodoAsync(id) is Todo todo
    ? Results.Ok(todo)
    : Results.NotFound())
    .Produces<Todo>(StatusCodes.Status200OK)
    .WithName("GetTodoItem")
    .WithTags("Getters");

app.MapPost("todos", async ([FromBody] Todo todo, ITodoRepository repository) => 
    {
        await repository.InsertTodoAsync(todo);
        await repository.SaveAsync();
        return Results.Created($"/hotels/{todo.Id}", todo);
    })
    .Accepts<Todo>("application/json")
    .Produces<Todo>(StatusCodes.Status201Created)
    .WithName("CreateTodoItem")
    .WithTags("Creators");

app.MapPut("/todos", async ([FromBody] Todo todo, ITodoRepository repository) =>
    {
        await repository.UpdateTodoAsync(todo);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<Todo>("application/json")
    .WithName("UpdateTodoItem")
    .WithTags("Updaters");

app.MapDelete("todos/{id}", async (int id, ITodoRepository repository) =>
    {
        await repository.DeleteTodoAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .WithName("DeleteTodoItem")
    .WithTags("Deleters");

app.MapGet("/todos/search/name/{query}",
    async (string query, ITodoRepository repository) =>
        await repository.GetTodosAsync(query) is IEnumerable<Todo> todos
        ? Results.Ok(todos)
        : Results.NotFound(Array.Empty<Todo>()))
    .Produces<List<Todo>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchTodoItems")
    .WithTags("Getters")
    .ExcludeFromDescription();

app.MapGet("/todos/search/iscomplete/{query}",
    async (bool query, ITodoRepository repository) =>
        await repository.GetTodosAsync(query) is IEnumerable<Todo> todos
        ? Results.Ok(todos)
        : Results.NotFound(Array.Empty<Todo>()))
    .Produces<List<Todo>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchTodoItemsFromBool")
    .WithTags("Getters")
    .ExcludeFromDescription();


app.UseHttpsRedirection();

app.Run();
