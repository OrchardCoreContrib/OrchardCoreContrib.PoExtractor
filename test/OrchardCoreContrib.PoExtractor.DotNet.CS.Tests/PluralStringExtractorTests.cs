using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.Tests;

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
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree
            .ParseText($@"{localizer}.Plural(1, ""{text}"", ""{pluralText}"");", path: "DummyPath");
        
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

    [Theory]
    [InlineData("S<ContainingType>.Plural")]
    [InlineData("T<ContainingType>.Plural")]
    public void ShouldExtractTypedStaticPluralString(string localizer)
    {
        // Arrange
        var text = "{0} thing";
        var pluralText = "{0} things";
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText($@"
namespace OrchardCore.Tests;

public class ContainingType
{{
    public void Run()
    {{
        {localizer}(1, ""{text}"", ""{pluralText}"");
    }}
}}
", path: "DummyPath");

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
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var extractor = new PluralStringExtractor(metadataProvider);

        var syntaxTree = CSharpSyntaxTree.ParseText($@"
namespace OrchardCore.Tests;

public class ContainingType
{{
    public void Run()
    {{
        SharedLocalizer.S.Plural(1, ""{text}"", ""{pluralText}"");
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
