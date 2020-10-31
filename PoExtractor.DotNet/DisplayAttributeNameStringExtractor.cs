using Microsoft.CodeAnalysis;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class DisplayAttributeNameStringExtractor : DisplayAttributeStringExtractor
    {
        public DisplayAttributeNameStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base("Name", metadataProvider)
        {
        }
    }
}
