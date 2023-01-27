using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders.Tests;

public class VisualBasicMetadataProviderTests
{
    [Fact]
    public void ContextShouldNotIgnoreInnerClass()
    {
        // Arrange
        var metadataProvider = new VisualBasicMetadataProvider("DummyBasePath");
        var codeText = @"Namespace OrchardCoreContrib.PoExtractor.Tests
    Public Class TestClass
        Public Property Model As InnerClass
    
        Public class InnerClass
            <Display(Name = ""Remember me?"")>
            Public Property RememberMe As Boolean
        End Class
    End Class
End Namespace";

        var syntaxTree = VisualBasicSyntaxTree.ParseText(codeText, path: "DummyPath");

        var node = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassBlockSyntax>()
            .Last()
            .ChildNodes()
            .First();

        // Act
        var context = metadataProvider.GetContext(node);

        // Assert
        Assert.Equal("OrchardCoreContrib.PoExtractor.Tests.TestClass.InnerClass", context);
    }
}
