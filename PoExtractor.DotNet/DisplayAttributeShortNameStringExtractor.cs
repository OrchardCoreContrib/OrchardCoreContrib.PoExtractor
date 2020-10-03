using Microsoft.CodeAnalysis;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class DisplayAttributeShortNameStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeShortNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("ShortName", metadataProvider)
        {
        }
    }
}
