using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.Tests;

public class SingularStringExtractorTests
{
    [Theory]
    [InlineData("S")]
    [InlineData("T")]
    public void ExtractIndexerString(string localizer)
    {
        // Arrange
        var text = "Thing";
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText($@"{localizer}[""{text}""];", path: "DummyPath");
        
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

    [Theory]
    [InlineData("S<ContainingType>(\"Thing\");")]
    [InlineData("T<ContainingType>(\"Thing\");")]
    public void ExtractTypedStaticLocalizerString(string code)
    {
        // Arrange
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText($@"
namespace OrchardCore.Tests;

public class ContainingType
{{
    public void Run()
    {{
        {code}
    }}
}}
", path: "DummyPath");

        var root = syntaxTree.GetRoot();
        SyntaxNode node = root.DescendantNodes().OfType<InvocationExpressionSyntax>().Single();

        // Act
        var extracted = extractor.TryExtract(node, out var result);

        // Assert
        Assert.True(extracted);
        Assert.Equal("Thing", result.Text);
        Assert.Equal("OrchardCore.Tests.ContainingType", result.Context);
    }

    [Fact]
    public void DoesNotExtractUntypedStaticLocalizerString()
    {
        // Arrange
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new SingularStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText($@"
namespace OrchardCore.Tests;

public class ContainingType
{{
    public void Run()
    {{
        SharedLocalizer.S(""Thing"");
    }}
}}
", path: "DummyPath");

        var node = syntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Single();

        // Act
        var extracted = extractor.TryExtract(node, out _);

        // Assert
        Assert.False(extracted);
    }
}
