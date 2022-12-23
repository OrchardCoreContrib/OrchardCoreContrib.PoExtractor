using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from <see cref="DisplayAttribute"/> Name property.
    /// </summary>
    public class DisplayAttributeNameStringExtractor : DisplayAttributeStringExtractor
    {
        /// <summary>
        /// Creates a new instanceof a <see cref="DisplayAttributeNameStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
        public DisplayAttributeNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("Name", metadataProvider)
        {
        }
    }
}
