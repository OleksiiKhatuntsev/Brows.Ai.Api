namespace Domain.Promtps;

public record PromptBody
{
    public string Prompt { get; init; } = string.Empty;
}
