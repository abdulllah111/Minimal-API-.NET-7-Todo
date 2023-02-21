public interface ITokenService{
    string BuildToken(string key, string issuerm, UserDto user);
}