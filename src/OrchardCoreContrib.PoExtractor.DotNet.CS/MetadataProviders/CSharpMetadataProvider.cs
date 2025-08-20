using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;

/// <summary>
/// Provides metadata for C# code files.
/// </summary>
public class CSharpMetadataProvider : IMetadataProvider<SyntaxNode>
{
    private readonly string _basePath;

    /// <summary>
    /// Creates a new instance of a <see cref="CSharpMetadataProvider"/>.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    public CSharpMetadataProvider(string basePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(basePath, nameof(basePath));

        _basePath = basePath;
    }

    /// <inheritdoc/>
    public string GetContext(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var @namespace = node.Ancestors()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault()?
            .Name.ToString();

        if (string.IsNullOrEmpty(@namespace))
        {
            @namespace = node.Ancestors()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .FirstOrDefault()?
                .Name.ToString();
        }

        var classes = node
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .Select(c => c.Identifier.ValueText);

        var @class = classes.Count() == 1
            ? classes.Single()
            : String.Join('.', classes.Reverse());

        return string.IsNullOrEmpty(@namespace)
            ? @class
            : $"{@namespace}.{@class}";
    }

    /// <inheritdoc/>
    public LocalizableStringLocation GetLocation(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var lineNumber = node
            .GetLocation()
            .GetMappedLineSpan()
            .StartLinePosition.Line;

        return new LocalizableStringLocation
        {
            SourceFileLine = lineNumber + 1,
            SourceFile = node.SyntaxTree.FilePath.TrimStart(_basePath),
            Comment = node.SyntaxTree.GetText().Lines[lineNumber].ToString().Trim()
        };
    }
}
