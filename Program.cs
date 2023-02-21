var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDbContext<TodoDb>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepository>(new UserRepository());
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    { 
        options.TokenValidationParameters = new(){
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
       

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    db.Database.EnsureCreated();
}

app.MapPost("/login", [AllowAnonymous] async ([FromBody] User user,
ITokenService tokenService, IUserRepository UserRepository) => {
    var userDto = UserRepository.GetUser(user);
    if (userDto == null) return Results.Unauthorized();
    var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
    builder.Configuration["Jwt:Issuer"], userDto);
    return Results.Ok(token);
});

app.MapGet("/todos", [Authorize] async (ITodoRepository repository) => 
    Results.Ok(await repository.GetTodosAsync()))
    .Produces<List<Todo>>(StatusCodes.Status200OK)
    .WithName("GetAllTodoItems")
    .WithTags("Getters");

app.MapGet("/todos/{id}",[Authorize] async (int id, ITodoRepository repository) => 
    await repository.GetTodoAsync(id) is Todo todo
    ? Results.Ok(todo)
    : Results.NotFound())
    .Produces<Todo>(StatusCodes.Status200OK)
    .WithName("GetTodoItem")
    .WithTags("Getters");

app.MapPost("todos",[Authorize] async ([FromBody] Todo todo, ITodoRepository repository) => 
    {
        await repository.InsertTodoAsync(todo);
        await repository.SaveAsync();
        return Results.Created($"/todos/{todo.Id}", todo);
    })
    .Accepts<Todo>("application/json")
    .Produces<Todo>(StatusCodes.Status201Created)
    .WithName("CreateTodoItem")
    .WithTags("Creators");

app.MapPut("/todos",[Authorize] async ([FromBody] Todo todo, ITodoRepository repository) =>
    {
        await repository.UpdateTodoAsync(todo);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<Todo>("application/json")
    .WithName("UpdateTodoItem")
    .WithTags("Updaters");

app.MapDelete("todos/{id}",[Authorize] async (int id, ITodoRepository repository) =>
    {
        await repository.DeleteTodoAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .WithName("DeleteTodoItem")
    .WithTags("Deleters");

app.MapGet("/todos/search/name/{query}",[Authorize]
    async (string query, ITodoRepository repository) =>
        await repository.GetTodosAsync(query) is IEnumerable<Todo> todos
        ? Results.Ok(todos)
        : Results.NotFound(Array.Empty<Todo>()))
    .Produces<List<Todo>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchTodoItems")
    .WithTags("Getters")
    .ExcludeFromDescription();

app.MapPost("/todos/search/iscomplete/{query}",[Authorize]
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
