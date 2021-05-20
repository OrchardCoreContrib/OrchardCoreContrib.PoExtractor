namespace PoExtractor.Core {
    /// <summary>
    /// Represents a location of the localizable string occurrence in the source code
    /// </summary>
    public class LocalizableStringLocation {
        /// <summary>
        /// Gets or sets the name of the source file
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the line number in the source file
        /// </summary>
        public int SourceFileLine { get; set; }

        /// <summary>
        /// Gets or sets a comment for the occurrence
        /// </summary>
        /// <remarks>
        /// Typically used to provide better understanding for translators, e.g. copy of the whole line from the source code
        /// </remarks>
        public string Comment { get; set; }
    }
}
