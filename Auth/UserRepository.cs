public class UserRepository : IUserRepository
{
    private List<UserDto> _users => new(){
        new UserDto("Admin", "123")
    };

    public UserDto GetUser(User user) =>
        _users.FirstOrDefault(u => 
            string.Equals(u.Login, user.Login) &&
            string.Equals(u.Password, user.Password)) ??
            throw new Exception();
}