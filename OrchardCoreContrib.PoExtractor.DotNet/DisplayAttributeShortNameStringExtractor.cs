using Microsoft.CodeAnalysis;
using OrchardCoreContrib.PoExtractor.Core.Contracts;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    public class DisplayAttributeShortNameStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeShortNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("ShortName", metadataProvider)
        {
        }
    }
}
