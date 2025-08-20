using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Extracts localizable string from <see cref="DisplayAttribute"/> Description property.
/// </summary>
/// <remarks>
/// Creates a new instance of a <see cref="DisplayAttributeDescriptionStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class DisplayAttributeDescriptionStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
    : DisplayAttributeStringExtractor("Description", metadataProvider)
{
}
