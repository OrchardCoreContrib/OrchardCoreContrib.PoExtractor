using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Traverses C# & VB AST and extracts localizable strings using provided collection of <see cref="IStringExtractor{TNode}"/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExtractingCodeWalker"/> class.
/// </remarks>
/// <param name="extractors">the collection of extractors to use</param>
/// <param name="strings">The <see cref="LocalizableStringCollection"/> where the results are saved.</param>
public class ExtractingCodeWalker(IEnumerable<IStringExtractor<SyntaxNode>> extractors, LocalizableStringCollection strings) : SyntaxWalker
{
    private readonly LocalizableStringCollection _strings = strings ?? throw new ArgumentNullException(nameof(strings));
    private readonly IEnumerable<IStringExtractor<SyntaxNode>> _extractors = extractors ?? throw new ArgumentNullException(nameof(extractors));

    /// <inheritdoc/>
    public override void Visit(SyntaxNode node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        base.Visit(node);

        foreach (var extractor in _extractors)
        {
            if (extractor.TryExtract(node, out var result))
            {
                _strings.Add(result);
            }
        }
    }
}
