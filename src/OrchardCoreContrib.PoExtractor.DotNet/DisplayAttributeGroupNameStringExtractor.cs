using Microsoft.CodeAnalysis;
using OrchardCoreContrib.PoExtractor;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from <see cref="DisplayAttribute"/> GroupName property.
    /// </summary>
    public class DisplayAttributeGroupNameStringExtractor : DisplayAttributeStringExtractor
    {
        /// <summary>
        /// Creates a new instance of a <see cref="DisplayAttributeGroupNameStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
        public DisplayAttributeGroupNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("GroupName", metadataProvider)
        {
        }
    }
}
