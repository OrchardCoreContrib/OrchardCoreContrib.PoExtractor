using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet.CS.MetadataProviders {
    /// <summary>
    /// Provides metadata for .cs code files
    /// </summary>
    public class CodeMetadataProvider : IMetadataProvider<SyntaxNode> {
        public string BasePath { get; private set; }

        public CodeMetadataProvider(string basePath) {
            this.BasePath = basePath;
        }

        public string GetContext(SyntaxNode node) {
            var @namespace = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
            var @class = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;

            return $"{@namespace}.{@class}";
        }

        public LocalizableStringLocation GetLocation(SyntaxNode node) {
            var lineNumber = node.GetLocation().GetMappedLineSpan().StartLinePosition.Line;

            return new LocalizableStringLocation {
                SourceFileLine = lineNumber + 1,
                SourceFile = node.SyntaxTree.FilePath.TrimStart(this.BasePath),
                Comment = node.SyntaxTree.GetText().Lines[lineNumber].ToString().Trim()
            };
        }
    }
}
