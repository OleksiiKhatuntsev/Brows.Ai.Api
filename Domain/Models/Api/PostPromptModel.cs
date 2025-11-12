using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Api;

public record PostPromptModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 500 characters")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Body is required")]
    [StringLength(10000, MinimumLength = 1, ErrorMessage = "Body must be between 1 and 10000 characters")]
    public string Body { get; init; } = string.Empty;
}

