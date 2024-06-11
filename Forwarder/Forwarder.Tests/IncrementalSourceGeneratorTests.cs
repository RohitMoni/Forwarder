using System.Linq;
using Forwarder.Tests.GeneratorTestData;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Forwarder.Tests;

public class IncrementalSourceGeneratorTests
{
    [Theory]
    [ClassData(typeof(BasicForward))]
    [ClassData(typeof(InternalForward))]
    [ClassData(typeof(NestedForward))]
    [ClassData(typeof(ParameterModifiers))]
    [ClassData(typeof(PublicAndInternalForward))]
    public void IncrementalGenerator_Generates_ExpectedSourceText(string[] originSource, string[] expectedGeneratedSource)
    {
        var generator = new IncrementalSourceGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = CSharpCompilation.Create(nameof(IncrementalSourceGeneratorTests),
            [..originSource.Select(s => CSharpSyntaxTree.ParseText(s))],
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // +1 to account for the generated attribute code file
        Assert.Equal(expectedGeneratedSource.Length + 1, runResult.GeneratedTrees.Length);
        var generatedFileSyntax = runResult.GeneratedTrees.Where(t => t.FilePath.EndsWith("_Forwarded.g.cs"));
        var generatedSourceText = generatedFileSyntax.Select(g => g.GetText().ToString()).ToArray();

        for (var i = 0; i < expectedGeneratedSource.Length; ++i)
        {
            Assert.Equal(expectedGeneratedSource[i], generatedSourceText[i], ignoreLineEndingDifferences: true);
        }
    }
}