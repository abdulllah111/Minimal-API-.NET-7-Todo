public class TodoApi : IApi
{
    public void Register(WebApplication app){
        
        app.MapGet("/todos", Get)
            .Produces<List<Todo>>(StatusCodes.Status200OK)
            .WithName("GetAllTodoItems")
            .WithTags("Getters");

        app.MapGet("/todos/{id}", GetById)
            .Produces<Todo>(StatusCodes.Status200OK)
            .WithName("GetTodoItem")
            .WithTags("Getters");

        app.MapPost("todos", Post)
            .Accepts<Todo>("application/json")
            .Produces<Todo>(StatusCodes.Status201Created)
            .WithName("CreateTodoItem")
            .WithTags("Creators");

        app.MapPut("/todos", Put)
            .Accepts<Todo>("application/json")
            .WithName("UpdateTodoItem")
            .WithTags("Updaters");

        app.MapDelete("todos/{id}", Delete)
            .WithName("DeleteTodoItem")
            .WithTags("Deleters");

        app.MapGet("/todos/search/name/{query}", GetByName)
            .Produces<List<Todo>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SearchTodoItems")
            .WithTags("Getters")
            .ExcludeFromDescription();

        app.MapPost("/todos/search/iscomplete/{query}", GetByComplete)
            .Produces<List<Todo>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SearchTodoItemsFromBool")
            .WithTags("Getters")
            .ExcludeFromDescription();
    }
    [Authorize] private async Task<IResult> Get(ITodoRepository repository) =>
        Results.Ok(await repository.GetTodosAsync());
    
    [Authorize] private async Task<IResult> GetById(int id, ITodoRepository repository) => 
        await repository.GetTodoAsync(id) is Todo todo
        ? Results.Ok(todo)
        : Results.NotFound();
    [Authorize] private async Task<IResult> Post([FromBody] Todo todo, ITodoRepository repository)
    {
        await repository.InsertTodoAsync(todo);
        await repository.SaveAsync();
        return Results.Created($"/todos/{todo.Id}", todo);
    }
    [Authorize] private async Task<IResult> Put([FromBody] Todo todo, ITodoRepository repository)
    {
        await repository.UpdateTodoAsync(todo);
        await repository.SaveAsync();
        return Results.NoContent();
    }
    [Authorize] private async Task<IResult> Delete(int id, ITodoRepository repository)
    {
        await repository.DeleteTodoAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    }
    [Authorize] private async Task<IResult> GetByName(string query, ITodoRepository repository) =>
        await repository.GetTodosAsync(query) is IEnumerable<Todo> todos
        ? Results.Ok(todos)
        : Results.NotFound(Array.Empty<Todo>());
    [Authorize] private async Task<IResult> GetByComplete(bool query, ITodoRepository repository) =>
        await repository.GetTodosAsync(query) is IEnumerable<Todo> todos
        ? Results.Ok(todos)
        : Results.NotFound(Array.Empty<Todo>());
}