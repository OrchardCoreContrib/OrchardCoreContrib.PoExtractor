using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.Tests;

public class SingularStringExtractorTests
{
    private readonly SingularStringExtractor extractor;
    
    public SingularStringExtractorTests()
    {
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        extractor = new SingularStringExtractor(metadataProvider);
    }
    
    [Fact]
    public void ExtractString()
    {
        // Arrange
        var text = "Thing";
        var syntaxTree = ParseSyntaxTree($"S[\"{text}\"];");
        
        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(2);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(text, result.Text);
    }

    [Fact]
    public void ExtractVerbatimMultilineString()
    {
        // Arrange
        var syntaxTree = ParseSyntaxTree(
            """
            S[@"Thing
                in a literal string"];
            """);
        
        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(2);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(
            $"Thing{Environment.NewLine}    in a literal string",
            result.Text);
    }

    [Fact]
    public void ExtractRawLiteralString()
    {
        // Arrange
        var syntaxTree = ParseSyntaxTree(
            """
            S[
                \"\"\"
                Thing
                in a literal string
                \"\"\"
            ];
            """);
        
        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(2);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(
            """
            Thing
            in a literal string
            """,
            result.Text);
    }

    private static SyntaxTree ParseSyntaxTree(string sourceCode)
    {
        return CSharpSyntaxTree.ParseText(
            sourceCode,
            path: "DummyPath");
    }
}