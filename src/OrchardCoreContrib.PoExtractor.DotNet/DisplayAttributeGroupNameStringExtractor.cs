using Microsoft.CodeAnalysis;
using OrchardCoreContrib.PoExtractor.Core.Contracts;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    public class DisplayAttributeGroupNameStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeGroupNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("GroupName", metadataProvider)
        {
        }
    }
}
