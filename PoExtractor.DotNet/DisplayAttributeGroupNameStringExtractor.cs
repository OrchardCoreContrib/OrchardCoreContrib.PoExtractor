using Microsoft.CodeAnalysis;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class DisplayAttributeGroupNameStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeGroupNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("GroupName", metadataProvider)
        {
        }
    }
}
