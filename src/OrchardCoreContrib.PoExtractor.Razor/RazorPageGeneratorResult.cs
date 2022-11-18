namespace OrchardCoreContrib.PoExtractor.Razor
{
    /// <summary>
    /// Represents a result for generated razor page.
    /// </summary>
    public class RazorPageGeneratorResult
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the razor enerated code.
        /// </summary>
        public string GeneratedCode { get; set; }
    }
}
