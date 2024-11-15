using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Extracts localizable string from <see cref="DisplayAttribute"/> Name property.
/// </summary>
/// <remarks>
/// Creates a new instanceof a <see cref="DisplayAttributeNameStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class DisplayAttributeNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
    : DisplayAttributeStringExtractor("Name", metadataProvider)
{
}
