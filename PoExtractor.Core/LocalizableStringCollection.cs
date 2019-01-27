using System.Collections.Generic;

namespace PoExtractor.Core {
    /// <summary>
    /// Represents collection of the all localizable strings in the project. Localizable strings with the same values are merged.
    /// </summary>
    public class LocalizableStringCollection {
        private Dictionary<string, LocalizableString> _values;

        /// <summary>
        /// Gets collection of all <see cref="LocalizableString"/> in the project
        /// </summary>
        public IEnumerable<LocalizableString> Values {
            get {
                return _values.Values;
            }
        }

        /// <summary>
        /// Creates a new empty instance of the <see cref="LocalizableStringCollection" /> class
        /// </summary>
        public LocalizableStringCollection() {
            _values = new Dictionary<string, LocalizableString>();
        }

        /// <summary>
        /// Adds <see cref="LocalizableStringOccurence"/> to the collection
        /// </summary>
        /// <param name="s">the item to add</param>
        public void Add(LocalizableStringOccurence s) {
            if (s != null) {
                var key = s.Context + s.Text;
                if (_values.TryGetValue(key, out var localizedString)) {
                    localizedString.Locations.Add(s.Location);
                } else {
                    _values.Add(key, new LocalizableString(s));
                }
            }
        }
    }
}
