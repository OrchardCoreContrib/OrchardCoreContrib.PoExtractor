using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;

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
        ArgumentException.ThrowIfNullOrEmpty(basePath, nameof(basePath));

        _basePath = basePath;
    }

    /// <inheritdoc/>
    public string GetContext(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var @namespace = node
            .Ancestors()
            .OfType<NamespaceBlockSyntax>()
            .FirstOrDefault()?.NamespaceStatement.Name
            .ToString();

        var classes = node
            .Ancestors()
            .OfType<ClassBlockSyntax>()
            .Select(c => c.ClassStatement.Identifier.ValueText);

        var @class = classes.Count() == 1
            ? classes.Single()
            : String.Join('.', classes.Reverse());

        return $"{@namespace}.{@class}";
    }

    /// <inheritdoc/>
    public LocalizableStringLocation GetLocation(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var lineNumber = node.GetLocation().GetMappedLineSpan().StartLinePosition.Line;

        return new LocalizableStringLocation
        {
            SourceFileLine = lineNumber + 1,
            SourceFile = node.SyntaxTree.FilePath.TrimStart(_basePath),
            Comment = node.SyntaxTree.GetText().Lines[lineNumber].ToString().Trim()
        };
    }
}
