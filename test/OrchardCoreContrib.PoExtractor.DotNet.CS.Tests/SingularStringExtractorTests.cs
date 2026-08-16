using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.Tests;

public class SingularStringExtractorTests
{
    [Theory]
    [InlineData("""S["Thing"];""", "Thing")]
    [InlineData(
        """
        S[@"This is a multi-line
        string."];
        """,
        "This is a multi-line\nstring.")]
    [InlineData("""S["my " + "text"];""", "my text")]
    [InlineData("""S["a " + "long " + "text"];""", "a long text")]
    [InlineData(
        """
        S["This is a long piece of text " +
          "continued on another line."];
        """,
        "This is a long piece of text continued on another line.")]
    public void ExtractString(string source, string expected)
    {
        // Arrange
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(2);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(expected, result.Text);
    }
}
