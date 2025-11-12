using Application.Mappers;
using Domain.Models.Api;
using FluentAssertions;
using Xunit;

namespace UnitTests.Application.Mappers;

/// <summary>
/// Unit tests for PromptMapper
/// </summary>
public class PromptMapperTests
{
    private readonly PromptMapper _mapper;

    public PromptMapperTests()
    {
        _mapper = new PromptMapper();
    }

    #region ToEntity(PostPromptModel) Tests

    [Fact]
    public void ToEntity_WithPostPromptModel_ShouldGenerateNewGuid()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test Title",
            Body = "Test Body"
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void ToEntity_WithPostPromptModel_ShouldMapTitleCorrectly()
    {
        // Arrange
        var expectedTitle = "My Prompt Title";
        var model = new PostPromptModel
        {
            Title = expectedTitle,
            Body = "Test Body"
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void ToEntity_WithPostPromptModel_ShouldMapBodyCorrectly()
    {
        // Arrange
        var expectedBody = "This is the prompt body";
        var model = new PostPromptModel
        {
            Title = "Test Title",
            Body = expectedBody
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Body.Should().Be(expectedBody);
    }

    [Fact]
    public void ToEntity_WithPostPromptModel_CalledMultipleTimes_ShouldGenerateDifferentGuids()
    {
        // Arrange
        var model1 = new PostPromptModel { Title = "Title1", Body = "Body1" };
        var model2 = new PostPromptModel { Title = "Title2", Body = "Body2" };

        // Act
        var result1 = _mapper.ToEntity(model1);
        var result2 = _mapper.ToEntity(model2);

        // Assert
        result1.Id.Should().NotBe(result2.Id);
    }

    #endregion

    #region ToEntity(UpdatePromptModel) Tests

    [Fact]
    public void ToEntity_WithUpdatePromptModel_ShouldUseProvidedId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var model = new UpdatePromptModel
        {
            Id = expectedId,
            Title = "Test Title",
            Body = "Test Body"
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Id.Should().Be(expectedId);
    }

    [Fact]
    public void ToEntity_WithUpdatePromptModel_ShouldMapTitleCorrectly()
    {
        // Arrange
        var expectedTitle = "Updated Title";
        var model = new UpdatePromptModel
        {
            Id = Guid.NewGuid(),
            Title = expectedTitle,
            Body = "Test Body"
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void ToEntity_WithUpdatePromptModel_ShouldMapBodyCorrectly()
    {
        // Arrange
        var expectedBody = "Updated body content";
        var model = new UpdatePromptModel
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Body = expectedBody
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Body.Should().Be(expectedBody);
    }

    [Fact]
    public void ToEntity_WithUpdatePromptModel_ShouldPreserveExactIdValue()
    {
        // Arrange
        var specificGuid = new Guid("12345678-1234-1234-1234-123456789012");
        var model = new UpdatePromptModel
        {
            Id = specificGuid,
            Title = "Test Title",
            Body = "Test Body"
        };

        // Act
        var result = _mapper.ToEntity(model);

        // Assert
        result.Id.Should().Be(specificGuid);
        result.Id.ToString().Should().Be("12345678-1234-1234-1234-123456789012");
    }

    #endregion
}

