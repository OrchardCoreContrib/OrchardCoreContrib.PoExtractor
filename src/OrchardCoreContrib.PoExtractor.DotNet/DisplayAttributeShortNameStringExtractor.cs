using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Extracts localizable string from <see cref="DisplayAttribute"/> ShortName property.
/// </summary>
/// <remarks>
/// Creates a new instance of a <see cref="DisplayAttributeShortNameStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class DisplayAttributeShortNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
    : DisplayAttributeStringExtractor("ShortName", metadataProvider)
{
}
