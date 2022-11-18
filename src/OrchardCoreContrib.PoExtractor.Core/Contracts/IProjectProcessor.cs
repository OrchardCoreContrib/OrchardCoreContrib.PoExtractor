using Microsoft.CodeAnalysis;

namespace OrchardCoreContrib.PoExtractor.Core.Contracts {
    public interface IProjectProcessor {
        void Process(string path, string basePath, LocalizableStringCollection strings);
    }
}
