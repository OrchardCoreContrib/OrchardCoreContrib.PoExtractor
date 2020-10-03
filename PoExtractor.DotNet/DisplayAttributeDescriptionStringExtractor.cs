using Microsoft.CodeAnalysis;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class DisplayAttributeDescriptionStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeDescriptionStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("Description", metadataProvider)
        {
        }
    }
}
