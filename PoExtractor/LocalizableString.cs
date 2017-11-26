using System.Collections.Generic;

namespace PoExtractor.Core {
    public class LocalizableString {
        public string Context { get; set; }
        public string Text { get; set; }
        public string TextPlural { get; set; }

        public List<LocalizableStringLocation> Locations { get; }

        public LocalizableString() {
            this.Locations = new List<LocalizableStringLocation>();
        }

        public LocalizableString(string text, string textPlural, string context, LocalizableStringLocation location) {
            this.Text = text;
            this.TextPlural = textPlural;
            this.Context = context;

            this.Locations = new List<LocalizableStringLocation>();
            this.Locations.Add(location);
        }
    }
}
