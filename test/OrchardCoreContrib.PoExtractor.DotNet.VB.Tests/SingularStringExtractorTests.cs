using Microsoft.CodeAnalysis.VisualBasic;
using OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.Tests;

public class SingularStringExtractorTests
{
    [Fact]
    public void ExtractString()
    {
        // Arrange
        var text = "Thing";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree.ParseText($"S(\"{text}\")", path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(1);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(text, result.Text);
    }
}