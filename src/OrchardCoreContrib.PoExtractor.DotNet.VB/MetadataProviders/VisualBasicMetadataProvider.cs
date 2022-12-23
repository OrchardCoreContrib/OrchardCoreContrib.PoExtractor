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
        private readonly string _basePath;

        /// <summary>
        /// Creates a new instance of a <see cref="VisualBasicMetadataProvider"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public VisualBasicMetadataProvider(string basePath)
        {
            _basePath = basePath;
        }

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
                SourceFile = node.SyntaxTree.FilePath.TrimStart(_basePath),
                Comment = node.SyntaxTree.GetText().Lines[lineNumber].ToString().Trim()
            };
        }
    }
}
