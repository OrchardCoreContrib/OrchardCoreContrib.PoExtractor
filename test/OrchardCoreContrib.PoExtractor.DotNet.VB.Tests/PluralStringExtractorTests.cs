using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.Tests;

public class PluralStringExtractorTests
{
    [Theory]
    [InlineData("S")]
    [InlineData("T")]
    public void ShouldExtractValidString(string localizer)
    {
        // Arrange
        var text = "{0} thing";
        var pluralText = "{0} things";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree
            .ParseText($@"{localizer}.Plural(1, ""{text}"", ""{pluralText}"")", path: "DummyPath");

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

    [Theory]
    [InlineData("S(Of ContainingType).Plural")]
    [InlineData("T(Of ContainingType).Plural")]
    public void ShouldExtractTypedStaticPluralString(string localizer)
    {
        // Arrange
        var text = "{0} thing";
        var pluralText = "{0} things";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree.ParseText($"""
            Namespace OrchardCore.Tests
                Public Class ContainingType
                    Public Sub Run()
                        {localizer}(1, "{text}", "{pluralText}")
                    End Sub
                End Class
            End Namespace
            """, path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single();

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal(text, result.Text);
        Assert.Equal(pluralText, result.TextPlural);
        Assert.Equal("OrchardCore.Tests.ContainingType", result.Context);
    }

    [Fact]
    public void ShouldNotExtractUntypedStaticPluralString()
    {
        // Arrange
        var text = "{0} thing";
        var pluralText = "{0} things";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree.ParseText($"""
            Namespace OrchardCore.Tests
                Public Class ContainingType
                    Public Sub Run()
                        SharedLocalizer.S.Plural(1, "{text}", "{pluralText}")
                    End Sub
                End Class
            End Namespace
            """, path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single();

        // Act
        var extracted = extractor.TryExtract(node, out _);

        // Assert
        Assert.False(extracted);
    }
}
