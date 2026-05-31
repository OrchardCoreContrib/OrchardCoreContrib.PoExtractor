using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.Razor.MetadataProviders;
using System.IO;
using Xunit;

namespace OrchardCoreContrib.PoExtractor.Razor.Tests
{
    public class RazorMetadataProviderIntegrationTests
    {
        [Fact]
        public void GetContext_RemovesProjectNameFromPath()
        {
            // Arrange
            // The test runs from the output directory, so use relative paths
            var outputDir = Directory.GetCurrentDirectory();
            var basePath = Path.Combine(outputDir, "Sample");
            var filePath = Path.Combine(basePath, "MyProject", "Views", "Home", "Index.cshtml");
            var code = File.ReadAllText(filePath);

            // Create a dummy syntax tree for the file
            var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath);
            var root = syntaxTree.GetRoot();

            var provider = new RazorMetadataProvider(basePath);

            // Act
            var context = provider.GetContext(root);

            // Assert
            Assert.Equal("Views.Home.Index", context);
        }
    }
}
