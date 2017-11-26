namespace PoExtractor.Core {
    public class LocalizableStringOccurence {
        public string Context { get; set; }
        public string Text { get; set; }
        public string TextPlural { get; set; }

        public LocalizableStringLocation Location { get; set; }
    }
}
