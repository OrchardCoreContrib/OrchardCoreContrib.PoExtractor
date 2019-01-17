using System.Collections.Generic;

namespace PoExtractor.Core {
    public class LocalizableStringCollection {
        private Dictionary<string, LocalizableString> _values;
        public IEnumerable<LocalizableString> Values {
            get {
                return _values.Values;
            }
        }

        public LocalizableStringCollection() {
            _values = new Dictionary<string, LocalizableString>();
        }

        public void Add(LocalizableStringOccurence s) {
            if (s != null) {
                var key = s.Context + s.Text;
                if (_values.TryGetValue(key, out var localizedString)) {
                    localizedString.Locations.Add(s.Location);
                } else {
                    _values.Add(key, new LocalizableString(s.Text, s.TextPlural, s.Context, s.Location));
                }
            }
        }
    }
}
