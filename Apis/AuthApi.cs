public class AuthApi : IApi
{
    public void Register(WebApplication app){
        app.MapPost("/login", [AllowAnonymous] ([FromBody] User user,
        ITokenService tokenService, IUserRepository UserRepository) =>
        {
            var userDto = UserRepository.GetUser(user);
            if (userDto == null) return Results.Unauthorized();
            var token = tokenService.BuildToken(app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"], userDto);
            return Results.Ok(token);
        });
    }
}