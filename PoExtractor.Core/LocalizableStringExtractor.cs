using Microsoft.CodeAnalysis;
using PoExtractor.Core.Contracts;

namespace PoExtractor.Core {
    public abstract class LocalizableStringExtractor<T> : IStringExtractor<T> {
        public IMetadataProvider<T> MetadataProvider { get; private set; }

        public LocalizableStringExtractor(IMetadataProvider<T> metadataProvider) {
            this.MetadataProvider = metadataProvider;
        }

        public abstract bool TryExtract(T node, out LocalizableStringOccurence result);

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
