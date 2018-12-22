using Microsoft.CodeAnalysis;

namespace PoExtractor.Core.Extractors {
    public abstract class LocalizableStringExtractor<T> : IStringExtractor<T> {
        public IMetadataProvider<T> MetadataProvider { get; private set; }

        public LocalizableStringExtractor(IMetadataProvider<T> metadataProvider) {
            this.MetadataProvider = metadataProvider;
        }

        public abstract LocalizableStringOccurence TryExtract(T node);

        protected LocalizableStringOccurence CreateLocalizedString(string text, string textPlural, T node) {
            var result = new LocalizableStringOccurence() {
                Text = text,
                TextPlural = textPlural,
                Location = this.MetadataProvider.GetLocation(node),
                Context = this.MetadataProvider.GetContext(node)
            };

            return result;
        }
    }
}
