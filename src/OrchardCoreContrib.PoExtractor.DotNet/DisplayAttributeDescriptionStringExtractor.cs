using Microsoft.CodeAnalysis;
using OrchardCoreContrib.PoExtractor;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    public class DisplayAttributeDescriptionStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeDescriptionStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("Description", metadataProvider)
        {
        }
    }
}
