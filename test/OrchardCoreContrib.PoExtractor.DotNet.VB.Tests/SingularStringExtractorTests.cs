using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.Tests;

public class SingularStringExtractorTests
{
    [Theory]
    [InlineData("S")]
    [InlineData("T")]
    public void ExtractString(string localizer)
    {
        // Arrange
        var text = "Thing";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree.ParseText($@"{localizer}(""{text}"")", path: "DummyPath");

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

    [Theory]
    [InlineData("S(Of ContainingType)(\"Thing\")")]
    [InlineData("T(Of ContainingType)(\"Thing\")")]
    public void ExtractTypedStaticString(string code)
    {
        // Arrange
        var text = "Thing";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree.ParseText($"""
            Namespace OrchardCore.Tests
                Public Class ContainingType
                    Public Sub Run()
                        {code}
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
        Assert.Equal("OrchardCore.Tests.ContainingType", result.Context);
    }

    [Fact]
    public void DoesNotExtractUntypedStaticString()
    {
        // Arrange
        var text = "Thing";
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = VisualBasicSyntaxTree.ParseText($"""
            Namespace OrchardCore.Tests
                Public Class ContainingType
                    Public Sub Run()
                        SharedLocalizer.S("{text}")
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
