using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from <see cref="DisplayAttribute"/> Description property.
    /// </summary>
    public class DisplayAttributeDescriptionStringExtractor : DisplayAttributeStringExtractor
    {
        /// <summary>
        /// Creates a new instance of a <see cref="DisplayAttributeDescriptionStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
        public DisplayAttributeDescriptionStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("Description", metadataProvider)
        {
        }
    }
}
