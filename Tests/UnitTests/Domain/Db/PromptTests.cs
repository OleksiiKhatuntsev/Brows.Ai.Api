using System.ComponentModel.DataAnnotations;
using Domain.Db;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain.Db;

/// <summary>
/// Tests for Prompt domain model to verify data annotations and model behavior.
/// </summary>
public class PromptTests
{
    #region Validation Tests

    [Fact]
    public void Validation_WithValidData_Passes()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Body = "Valid Body"
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void Validation_WithNullTitle_Fails()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = null!,
            Body = "Valid Body"
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains("Title"));
    }

    [Fact]
    public void Validation_WithEmptyTitle_Fails()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "",
            Body = "Valid Body"
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains("Title"));
    }

    [Fact]
    public void Validation_WithNullBody_Fails()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Body = null!
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains("Body"));
    }

    [Fact]
    public void Validation_WithEmptyBody_Fails()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Body = ""
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains("Body"));
    }

    [Fact]
    public void Validation_WithTitleExceedingMaxLength_FailsWithMessage()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = new string('A', 501), // 501 characters (max is 500)
            Body = "Valid Body"
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains("Title"));
        results.First(r => r.MemberNames.Contains("Title")).ErrorMessage
            .Should().Contain("500");
    }

    [Fact]
    public void Validation_WithBodyExceedingMaxLength_FailsWithMessage()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Body = new string('B', 10001) // 10001 characters (max is 10000)
        };

        // Act
        var context = new ValidationContext(prompt);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(prompt, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle(r => r.MemberNames.Contains("Body"));
        results.First(r => r.MemberNames.Contains("Body")).ErrorMessage
            .Should().Contain("10000");
    }

    #endregion

    #region Model Behavior Tests

    [Fact]
    public void Constructor_WithDefaultValues_GeneratesNewGuid()
    {
        // Arrange & Act
        var prompt = new Prompt();

        // Assert
        prompt.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_WithDefaultValues_SetsEmptyStrings()
    {
        // Arrange & Act
        var prompt = new Prompt();

        // Assert
        prompt.Title.Should().BeEmpty();
        prompt.Body.Should().BeEmpty();
    }

    [Fact]
    public void Initializer_WithProvidedValues_SetsAllProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Test Title";
        var body = "Test Body";

        // Act
        var prompt = new Prompt
        {
            Id = id,
            Title = title,
            Body = body
        };

        // Assert
        prompt.Id.Should().Be(id);
        prompt.Title.Should().Be(title);
        prompt.Body.Should().Be(body);
    }

    [Fact]
    public void RecordEquality_WithIdenticalValues_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var prompt1 = new Prompt
        {
            Id = id,
            Title = "Test Title",
            Body = "Test Body"
        };
        var prompt2 = new Prompt
        {
            Id = id,
            Title = "Test Title",
            Body = "Test Body"
        };

        // Act & Assert
        prompt1.Should().Be(prompt2);
        prompt1.GetHashCode().Should().Be(prompt2.GetHashCode());
    }

    [Fact]
    public void RecordEquality_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var prompt1 = new Prompt
        {
            Id = id,
            Title = "Test Title",
            Body = "Test Body"
        };
        var prompt2 = new Prompt
        {
            Id = id,
            Title = "Different Title",
            Body = "Test Body"
        };

        // Act & Assert
        prompt1.Should().NotBe(prompt2);
    }

    #endregion
}
