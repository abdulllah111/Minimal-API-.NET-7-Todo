var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

Configure(app);

var apis = app.Services.GetServices<IApi>();
foreach(var api in apis){
    if (api is null) throw new InvalidProgramException("Api not found");
    api.Register(app);
}

app.Run();

void RegisterServices(IServiceCollection servicess){
    servicess.AddEndpointsApiExplorer();
    servicess.AddSwaggerGen();

    servicess.AddDatabaseDeveloperPageExceptionFilter();
    servicess.AddDbContext<TodoDb>(options => {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    servicess.AddSingleton<ITokenService>(new TokenService());
    servicess.AddSingleton<IUserRepository>(new UserRepository());
    servicess.AddScoped<ITodoRepository, TodoRepository>();
    servicess.AddAuthorization();
    servicess.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    servicess.AddTransient<IApi, TodoApi>();
    servicess.AddTransient<IApi, AuthApi>();
}

void Configure(WebApplication app){
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment()){
        app.UseSwagger();
        app.UseSwaggerUI();
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
        db.Database.EnsureCreated();
    }
    app.UseHttpsRedirection();
}