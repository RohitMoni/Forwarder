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
    [ClassData(typeof(ParameterModifiers))]
    public void IncrementalGenerator_Generates_ExpectedSourceText(string originSource, string expectedGeneratedSource)
    {
        var generator = new IncrementalSourceGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = CSharpCompilation.Create(nameof(IncrementalSourceGeneratorTests),
            [CSharpSyntaxTree.ParseText(originSource)],
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        // Run generators and retrieve all results.
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // All generated files can be found in 'RunResults.GeneratedTrees'.
        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("_Forwarded.g.cs"));
        var generatedSourceText = generatedFileSyntax.GetText().ToString();

        // Complex generators should be tested using text comparison.
        Assert.Equal(expectedGeneratedSource, generatedSourceText, ignoreLineEndingDifferences: true);
    }
}