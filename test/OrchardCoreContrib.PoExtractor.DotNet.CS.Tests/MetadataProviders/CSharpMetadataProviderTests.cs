using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders.Tests;

public class CSharpMetadataProviderTests
{
    [Fact]
    public void ContextShouldNotIgnoreInnerClass()
    {
        // Arrange
        var metadataProvider = new CSharpMetadataProvider("DummyBasePath");
        var codeText = @"namespace OrchardCoreContrib.PoExtractor.Tests

public class TestClass
{
    public InnerClass Model { get; set; }
    
    public class InnerClass
    {
        [Display(Name = ""Remember me?"")]
        public bool RememberMe { get; set; }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(codeText, path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Last()
            .ChildNodes()
            .First();

        // Act
        var context = metadataProvider.GetContext(node);

        // Assert
        Assert.Equal("OrchardCoreContrib.PoExtractor.Tests.TestClass.InnerClass", context);
    }
}
