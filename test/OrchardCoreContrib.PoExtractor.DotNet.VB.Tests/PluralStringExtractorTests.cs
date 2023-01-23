using Microsoft.CodeAnalysis.VisualBasic;
using OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.Tests;

public class PluralStringExtractorTests
{
    [Fact]
    public void ShouldExtractValidString()
    {
        // Arrange
        var text = "{0} thing";
        var pluralText = "{0} things";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree
            .ParseText($"S.Plural(1, \"{text}\", \"{pluralText}\")", path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .ElementAt(1);

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(text, result.Text);
        Assert.Equal(pluralText, result.TextPlural);
    }
}