using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders
{
    /// <summary>
    /// Provides metadata for .cs code files
    /// </summary>
    public class CodeMetadataProvider : IMetadataProvider<SyntaxNode>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="CodeMetadataProvider"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public CodeMetadataProvider(string basePath)
        {
            this.BasePath = basePath;
        }

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        public string BasePath { get; private set; }

        /// <inheritdoc/>
        public string GetContext(SyntaxNode node)
        {
            var @namespace = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
            var @class = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;

            return $"{@namespace}.{@class}";
        }

        /// <inheritdoc/>
        public LocalizableStringLocation GetLocation(SyntaxNode node)
        {
            var lineNumber = node.GetLocation().GetMappedLineSpan().StartLinePosition.Line;

            return new LocalizableStringLocation
            {
                SourceFileLine = lineNumber + 1,
                SourceFile = node.SyntaxTree.FilePath.TrimStart(this.BasePath),
                Comment = node.SyntaxTree.GetText().Lines[lineNumber].ToString().Trim()
            };
        }
    }
}
