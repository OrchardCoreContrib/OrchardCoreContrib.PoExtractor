using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Extracts localizable string from <see cref="DisplayAttribute"/> GroupName property.
/// </summary>
/// <remarks>
/// Creates a new instance of a <see cref="DisplayAttributeGroupNameStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class DisplayAttributeGroupNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
    : DisplayAttributeStringExtractor("GroupName", metadataProvider)
{
}
