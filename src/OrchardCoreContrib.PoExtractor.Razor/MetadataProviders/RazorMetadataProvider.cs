using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.Razor.MetadataProviders;

/// <summary>
/// Provides metadata for Razor .cshtml files.
/// </summary>
public class RazorMetadataProvider : IMetadataProvider<SyntaxNode>
{
    private static readonly string _razorPageExtension = ".cshtml";
    private static readonly string _razorComponentExtension = ".razor";

    private string[] _sourceCache;
    private string _sourceCachePath;

    private readonly string _basePath;

    /// <summary>
    /// Creates a new instance of a <see cref="RazorMetadataProvider"/>.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    public RazorMetadataProvider(string basePath)
    {
        _basePath = basePath;
    }

    /// <inheritdoc/>
    public string GetContext(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var path = node.SyntaxTree.FilePath.TrimStart(_basePath);
        path = RemoveRazorFileExtension(path);

        return path.Replace(Path.DirectorySeparatorChar, '.');
    }

    private static string RemoveRazorFileExtension(string path)
    {
        return path
            .Replace(_razorPageExtension, string.Empty)
            .Replace(_razorComponentExtension, string.Empty);
    }

    /// <inheritdoc/>
    public LocalizableStringLocation GetLocation(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var result = new LocalizableStringLocation
        {
            SourceFile = node.SyntaxTree.FilePath.TrimStart(_basePath)
        };

        var statement = node
            .Ancestors()
            .OfType<ExpressionStatementSyntax>()
            .FirstOrDefault();

        if (statement != null)
        {
            var lineTriviaSyntax = statement
                .DescendantTrivia()
                .OfType<SyntaxTrivia>()
                .Where(o => o.IsKind(SyntaxKind.LineDirectiveTrivia) && o.HasStructure)
                .FirstOrDefault();

            if (lineTriviaSyntax.GetStructure() is LineDirectiveTriviaSyntax lineTrivia && lineTrivia.HashToken.Text == "#" && lineTrivia.DirectiveNameToken.Text == "line")
            {
                if (int.TryParse(lineTrivia.Line.Text, out var lineNumber))
                {
                    result.SourceFileLine = lineNumber;
                    result.Comment = GetSourceCodeLine(node.SyntaxTree.FilePath, lineNumber)?.Trim();
                }
            }
        }

        return result;
    }

    private string GetSourceCodeLine(string path, int line)
    {
        if (_sourceCachePath != path)
        {
            _sourceCache = null;
            _sourceCachePath = null;

            try
            {
                _sourceCache = File.ReadAllLines(path);
                _sourceCachePath = path;
            }
            catch
            {
            }
        }

        var zeroBasedLineNumber = line - 1;
        if (_sourceCache != null && _sourceCache.Length > zeroBasedLineNumber)
        {
            return _sourceCache[zeroBasedLineNumber];
        }

        return null;
    }
}
