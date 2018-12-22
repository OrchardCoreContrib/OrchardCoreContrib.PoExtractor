using Microsoft.CodeAnalysis;

namespace PoExtractor.Core {
    public interface IMetadataProvider<T> {
        string GetContext(T node);
        LocalizableStringLocation GetLocation(T node);
    }
}
