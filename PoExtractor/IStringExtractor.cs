using Microsoft.CodeAnalysis;

namespace PoExtractor.Core {
    public interface IStringExtractor<T> {
        LocalizableStringOccurence TryExtract(T node);
    }
}
