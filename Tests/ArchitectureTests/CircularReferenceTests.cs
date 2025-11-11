using System.Reflection;
using Xunit;

namespace ArchitectureTests;

/// <summary>
/// Tests to verify there are no circular dependencies between projects.
/// </summary>
public class CircularReferenceTests
{
    [Fact]
    public void Domain_ShouldNotHave_CircularReferences()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        // Act & Assert
        Assert.DoesNotContain(referencedAssemblies, a =>
            a.Name == "Infrastructure" ||
            a.Name == "Api" ||
            a.Name == "Brows.Ai.Api");
    }

    [Fact]
    public void Infrastructure_ShouldNotHave_CircularReferencesWithApi()
    {
        // Arrange
        var infrastructureAssembly = typeof(Infrastructure.Data.BrowsAiDbContext).Assembly;
        var referencedAssemblies = infrastructureAssembly.GetReferencedAssemblies();

        // Act & Assert
        Assert.DoesNotContain(referencedAssemblies, a =>
            a.Name == "Api" ||
            a.Name == "Brows.Ai.Api");
    }

    [Fact]
    public void AllProjects_ShouldHaveAcyclicDependencyGraph()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;
        var infrastructureAssembly = typeof(Infrastructure.Data.BrowsAiDbContext).Assembly;
        var apiAssembly = typeof(Program).Assembly;

        var assemblies = new[]
        {
            new { Name = "Domain", Assembly = domainAssembly },
            new { Name = "Infrastructure", Assembly = infrastructureAssembly },
            new { Name = "Api", Assembly = apiAssembly }
        };

        // Act - Check for any circular dependencies
        var circularDependencies = new List<string>();

        foreach (var assembly in assemblies)
        {
            var referencedAssemblies = assembly.Assembly.GetReferencedAssemblies();
            var projectRefs = referencedAssemblies
                .Where(a => a.Name == "Domain" || a.Name == "Infrastructure" || a.Name == "Api" || a.Name == "Brows.Ai.Api")
                .ToList();

            foreach (var refAssembly in projectRefs)
            {
                // Check if the referenced assembly references back
                var refAssemblyLoaded = assemblies.FirstOrDefault(a =>
                    a.Name == refAssembly.Name ||
                    a.Assembly.GetName().Name == refAssembly.Name);

                if (refAssemblyLoaded != null)
                {
                    var nestedRefs = refAssemblyLoaded.Assembly.GetReferencedAssemblies();
                    if (nestedRefs.Any(nr => nr.Name == assembly.Name || nr.Name == assembly.Assembly.GetName().Name))
                    {
                        circularDependencies.Add($"{assembly.Name} <-> {refAssembly.Name}");
                    }
                }
            }
        }

        // Assert
        Assert.Empty(circularDependencies);
    }

    [Fact]
    public void DependencyChain_ShouldBeCorrect()
    {
        // Expected dependency chain: Api -> Infrastructure -> Domain
        // This test verifies the expected architecture is maintained

        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;
        var infrastructureAssembly = typeof(Infrastructure.Data.BrowsAiDbContext).Assembly;
        var apiAssembly = typeof(Program).Assembly;

        var domainRefs = domainAssembly.GetReferencedAssemblies()
            .Where(a => a.Name == "Domain" || a.Name == "Infrastructure" || a.Name == "Api" || a.Name == "Brows.Ai.Api")
            .ToList();

        var infrastructureRefs = infrastructureAssembly.GetReferencedAssemblies()
            .Where(a => a.Name == "Domain" || a.Name == "Infrastructure" || a.Name == "Api" || a.Name == "Brows.Ai.Api")
            .ToList();

        var apiRefs = apiAssembly.GetReferencedAssemblies()
            .Where(a => a.Name == "Domain" || a.Name == "Infrastructure" || a.Name == "Api" || a.Name == "Brows.Ai.Api")
            .ToList();

        // Assert
        // Domain should have no project dependencies
        Assert.Empty(domainRefs);

        // Infrastructure should only reference Domain
        Assert.Single(infrastructureRefs);
        Assert.Equal("Domain", infrastructureRefs.First().Name);

        // Api should reference Infrastructure (and possibly Domain)
        Assert.Contains(apiRefs, a => a.Name == "Infrastructure");
    }

    [Fact]
    public void NoProject_ShouldReferenceItself()
    {
        // Arrange
        var domainAssembly = typeof(Domain.Db.Prompt).Assembly;
        var infrastructureAssembly = typeof(Infrastructure.Data.BrowsAiDbContext).Assembly;
        var apiAssembly = typeof(Program).Assembly;

        var assemblies = new[]
        {
            new { Name = "Domain", Assembly = domainAssembly },
            new { Name = "Infrastructure", Assembly = infrastructureAssembly },
            new { Name = "Api", Assembly = apiAssembly }
        };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var referencedAssemblies = assembly.Assembly.GetReferencedAssemblies();
            var selfReference = referencedAssemblies.Any(a =>
                a.Name == assembly.Name ||
                a.Name == assembly.Assembly.GetName().Name);

            Assert.False(selfReference, $"{assembly.Name} should not reference itself");
        }
    }
}

