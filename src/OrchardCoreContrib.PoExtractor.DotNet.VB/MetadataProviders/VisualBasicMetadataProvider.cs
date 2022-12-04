using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders
{
    /// <summary>
    /// Provides metadata for .vb code files
    /// </summary>
    public class VisualBasicMetadataProvider : IMetadataProvider<SyntaxNode>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="VisualBasicMetadataProvider"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public VisualBasicMetadataProvider(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Gets a base path.
        /// </summary>
        public string BasePath { get; }

        /// <inheritdoc/>
        public string GetContext(SyntaxNode node)
        {
            var @namespace = node.Ancestors().OfType<NamespaceStatementSyntax>().FirstOrDefault()?.Name.ToString();
            var @class = node.Ancestors().OfType<ClassStatementSyntax>().FirstOrDefault()?.Identifier.ValueText;

            return $"{@namespace}.{@class}";
        }

        /// <inheritdoc/>
        public LocalizableStringLocation GetLocation(SyntaxNode node)
        {
            var lineNumber = node.GetLocation().GetMappedLineSpan().StartLinePosition.Line;

            return new LocalizableStringLocation
            {
                SourceFileLine = lineNumber + 1,
                SourceFile = node.SyntaxTree.FilePath.TrimStart(BasePath),
                Comment = node.SyntaxTree.GetText().Lines[lineNumber].ToString().Trim()
            };
        }
    }
}
