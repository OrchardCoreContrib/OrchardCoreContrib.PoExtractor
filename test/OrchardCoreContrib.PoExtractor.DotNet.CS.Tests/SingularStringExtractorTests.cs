using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.Tests;

public class SingularStringExtractorTests
{
    [Fact]
    public void ExtractString()
    {
        // Arrange
        var text = "Thing";
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText($"S[\"{text}\"];", path: "DummyPath");
        
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
}