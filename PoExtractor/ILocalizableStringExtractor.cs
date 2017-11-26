using Microsoft.CodeAnalysis;

namespace PoExtractor.Core {
    public interface ILocalizableStringExtractor {
        LocalizableStringOccurence TryExtract(SyntaxNode node);
    }
}
