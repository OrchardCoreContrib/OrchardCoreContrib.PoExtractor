namespace PoExtractor.Core {
    /// <summary>
    /// Represents the specific occurence of the localizable string in the project
    /// </summary>
    public class LocalizableStringOccurence {
        public string Context { get; set; }
        public string Text { get; set; }
        public string TextPlural { get; set; }

        public LocalizableStringLocation Location { get; set; }
    }
}
