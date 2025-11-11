using System.ComponentModel.DataAnnotations;

namespace Domain.Db;

public record Prompt
{
    [Key]
    [Required]
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string Body { get; init; } = string.Empty;
}
