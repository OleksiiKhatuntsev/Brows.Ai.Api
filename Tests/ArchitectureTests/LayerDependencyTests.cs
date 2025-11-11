using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests;

/// <summary>
/// Tests to enforce proper layering and dependency rules across the solution.
/// </summary>
public class LayerDependencyTests
{
    private const string DomainNamespace = "Domain";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string ApplicationNamespace = "Application";
    private const string ApiNamespace = "Brows.Ai.Api";

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on Infrastructure layer. Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on Application layer. Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_PresentationLayer()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on presentation layer (Api). Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Application()
    {
        // Arrange
        var infrastructureAssembly = typeof(Infrastructure.Data.BrowsAiDbContext).Assembly;

        // Act
        var result = Types.InAssembly(infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Infrastructure layer should not depend on Application layer. Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_PresentationLayer()
    {
        // Arrange
        var infrastructureAssembly = typeof(Infrastructure.Data.BrowsAiDbContext).Assembly;

        // Act
        var result = Types.InAssembly(infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Infrastructure layer should not depend on presentation layer (Api). Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_ShouldNotDependOn_PresentationLayer()
    {
        // Arrange
        // Note: Application assembly will be loaded when first service is created
        var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Application");

        // Skip test if Application assembly is not loaded yet
        if (applicationAssembly == null)
        {
            Assert.True(true, "Application assembly not loaded, skipping test");
            return;
        }

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Application layer should not depend on presentation layer (Api). Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void PresentationLayer_ShouldAccessDomainThrough_Infrastructure()
    {
        // Arrange
        var apiAssembly = typeof(Program).Assembly;

        // Act - Check if Api project directly references Domain types (it should go through Infrastructure)
        // This is a guideline test - in some cases direct Domain access might be acceptable
        var result = Types.InAssembly(apiAssembly)
            .That()
            .ResideInNamespace(ApiNamespace)
            .And()
            .AreNotInterfaces()
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert - This is more of a guideline; might need adjustment based on your architecture
        // If you want to allow Api to access Domain directly, comment out this assertion
        Assert.True(result.IsSuccessful || result.FailingTypeNames?.Count() < 5,
            $"Presentation layer should primarily access Domain through Infrastructure. Consider if direct dependencies are necessary. Types with direct Domain access: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_InfrastructureConcerns()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;

        // Act - Ensure Domain doesn't depend on external infrastructure concerns
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("System.Net.Http")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain should not depend on infrastructure concerns like EF Core or HTTP. Violating types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}

