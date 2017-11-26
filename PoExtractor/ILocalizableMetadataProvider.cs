using Microsoft.CodeAnalysis;

namespace PoExtractor.Core {
    public interface ILocalizableMetadataProvider {
        string GetContext(SyntaxNode node);
        LocalizableStringLocation GetLocation(SyntaxNode node);
    }
}
