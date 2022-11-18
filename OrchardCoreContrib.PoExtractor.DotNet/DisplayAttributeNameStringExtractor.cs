using Microsoft.CodeAnalysis;
using OrchardCoreContrib.PoExtractor.Core.Contracts;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    public class DisplayAttributeNameStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("Name", metadataProvider)
        {
        }
    }
}
