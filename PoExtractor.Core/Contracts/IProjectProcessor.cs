using Microsoft.CodeAnalysis;

namespace PoExtractor.Core.Contracts {
    public interface IProjectProcessor {
        void Process(string path, string basePath, LocalizableStringCollection strings);
    }
}
