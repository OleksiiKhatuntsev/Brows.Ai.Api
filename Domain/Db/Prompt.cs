namespace Domain.Db;

public record Prompt
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Title { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;
}
