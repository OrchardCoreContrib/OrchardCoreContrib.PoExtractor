using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from <see cref="DisplayAttribute"/> ShortName property.
    /// </summary>
    public class DisplayAttributeShortNameStringExtractor : DisplayAttributeStringExtractor
    {
        /// <summary>
        /// Creates a new instance of a <see cref="DisplayAttributeShortNameStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
        public DisplayAttributeShortNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("ShortName", metadataProvider)
        {
        }
    }
}
