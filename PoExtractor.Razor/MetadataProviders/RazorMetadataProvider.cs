using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.Razor.MetadataProviders {
    /// <summary>
    /// Provides metadata for Razor .cshtml files
    /// </summary>
    public class RazorMetadataProvider : IMetadataProvider<SyntaxNode> {
        private string[] _sourceCache;
        private string _sourceCachePath;

        public string BasePath { get; set; }

        public RazorMetadataProvider(string basePath) {
            this.BasePath = basePath;
        }

        public string GetContext(SyntaxNode node) {
            var path = node.SyntaxTree.FilePath.TrimStart(this.BasePath);
            return path.Replace(Path.DirectorySeparatorChar, '.').Replace(".cshtml", string.Empty);
        }

        public LocalizableStringLocation GetLocation(SyntaxNode node) {
            var result = new LocalizableStringLocation {
                SourceFile = node.SyntaxTree.FilePath.TrimStart(this.BasePath)
            };

            var statement = node.Ancestors().OfType<ExpressionStatementSyntax>().FirstOrDefault();
            if (statement != null) {
                var lineTriviaSyntax = statement.DescendantTrivia().OfType<SyntaxTrivia>().Where(o => o.IsKind(SyntaxKind.LineDirectiveTrivia) && o.HasStructure).FirstOrDefault();
                if (lineTriviaSyntax.GetStructure() is LineDirectiveTriviaSyntax lineTrivia && lineTrivia.HashToken.Text == "#" && lineTrivia.DirectiveNameToken.Text == "line") {
                    if (int.TryParse(lineTrivia.Line.Text, out var lineNumber)) {
                        result.SourceFileLine = lineNumber;
                        result.Comment = this.GetSourceCodeLine(node.SyntaxTree.FilePath, lineNumber).Trim();
                    }
                }
            }

            return result;
        }

        private string GetSourceCodeLine(string path, int line) {
            if (_sourceCachePath != path) {
                _sourceCache = null;
                _sourceCachePath = null;

                try {
                    _sourceCache = File.ReadAllLines(path);
                    _sourceCachePath = path;
                } catch {
                    ; // do nothing
                }
            }

            var zeroBasedLineNumber = line - 1;
            if (_sourceCache != null && _sourceCache.Length > zeroBasedLineNumber) {
                return _sourceCache[zeroBasedLineNumber];
            }

            return null;
        }
    }
}
