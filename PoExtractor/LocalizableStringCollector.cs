using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace PoExtractor.Core {
    public class LocalizableStringCollector : SyntaxWalker {
        private Dictionary<string, LocalizableString> _strings;
        public IEnumerable<LocalizableString> Strings {
            get {
                return _strings.Values;
            }
        }

        public IEnumerable<ILocalizableStringExtractor> Extractors { get; set; }

        public string DefaultContext { get; set; }

        public LocalizableStringCollector(IEnumerable<ILocalizableStringExtractor> extractors) {
            _strings = new Dictionary<string, LocalizableString>();
            this.Extractors = extractors;
        }

        public override void Visit(SyntaxNode node) {
            base.Visit(node);

            foreach (var extractor in Extractors) {
                var translatableString = extractor.TryExtract(node);
                if (translatableString != null) {
                    var key = translatableString.Context + translatableString.Text;
                    if (_strings.TryGetValue(key, out var localizedString)) {
                        localizedString.Locations.Add(translatableString.Location);
                    } else {
                        _strings.Add(key, new LocalizableString(translatableString.Text, translatableString.TextPlural, translatableString.Context, translatableString.Location));
                    }
                }
            }
        }
    }
}
