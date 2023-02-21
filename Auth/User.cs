public record UserDto(string Login, string Password);

public record User{
    public int Id { get; set; }
    [Required]
    public string Login { get; set; } = default!;
    [Required]
    public string Password { get; set; } = default!;
}