using Microsoft.CodeAnalysis;

namespace PoExtractor.Core.Extractors {
    public abstract class LocalizableStringExtractor : ILocalizableStringExtractor {
        public ILocalizableMetadataProvider MetadataProvider { get; private set; }

        public LocalizableStringExtractor(ILocalizableMetadataProvider metadataProvider) {
            this.MetadataProvider = metadataProvider;
        }

        public abstract LocalizableStringOccurence TryExtract(SyntaxNode node);

        protected LocalizableStringOccurence CreateLocalizedString(string text, string textPlural, SyntaxNode node) {
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
