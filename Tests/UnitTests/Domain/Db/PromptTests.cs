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
    #region Data Annotation Tests

    [Fact]
    public void Prompt_Validation_WithValidData_ShouldPass()
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
    public void Prompt_Validation_WithEmptyTitle_ShouldFail()
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
    public void Prompt_Validation_WithEmptyBody_ShouldFail()
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
    public void Prompt_Validation_WithTitleTooLong_ShouldFail()
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
    public void Prompt_Validation_WithBodyTooLong_ShouldFail()
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

    [Fact]
    public void Prompt_Validation_WithNullTitle_ShouldFail()
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
    public void Prompt_Validation_WithNullBody_ShouldFail()
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

    #endregion

    #region Model Behavior Tests

    [Fact]
    public void Prompt_DefaultConstructor_ShouldGenerateNewGuid()
    {
        // Act
        var prompt = new Prompt();

        // Assert
        prompt.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Prompt_DefaultConstructor_ShouldHaveEmptyStrings()
    {
        // Act
        var prompt = new Prompt();

        // Assert
        prompt.Title.Should().BeEmpty();
        prompt.Body.Should().BeEmpty();
    }

    [Fact]
    public void Prompt_WithInitializer_ShouldSetProperties()
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
    public void Prompt_RecordEquality_ShouldWorkCorrectly()
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
        var prompt3 = new Prompt
        {
            Id = id,
            Title = "Different Title",
            Body = "Test Body"
        };

        // Assert - Records with same values should be equal
        prompt1.Should().Be(prompt2);
        prompt1.GetHashCode().Should().Be(prompt2.GetHashCode());

        // Assert - Records with different values should not be equal
        prompt1.Should().NotBe(prompt3);
    }

    #endregion
}

