public record Todo 
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsComplete { get; set; } = default!;
}