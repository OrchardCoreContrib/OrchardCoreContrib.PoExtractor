using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.Tests;

public class PluralStringExtractorTests
{
    [Fact]
    public void ShouldExtractValidString()
    {
        // Arrange
        var text = "{0} thing";
        var pluralText = "{0} things";
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree
            .ParseText($"S.Plural(1, \"{text}\", \"{pluralText}\");", path: "DummyPath");
        
        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(2);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(text, result.Text);
        Assert.Equal(pluralText, result.TextPlural);
    }
}