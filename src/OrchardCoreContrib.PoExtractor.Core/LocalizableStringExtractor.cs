namespace OrchardCoreContrib.PoExtractor
{
    /// <summary>
    /// Represents a base class for extracting a localizable strings.
    /// </summary>
    /// <typeparam name="T">The type of the node.</typeparam>
    public abstract class LocalizableStringExtractor<T> : IStringExtractor<T>
    {
        public IMetadataProvider<T> MetadataProvider { get; private set; }

        /// <summary>
        /// Creates a new instance of a <see cref="LocalizableStringExtractor{T}"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
        public LocalizableStringExtractor(IMetadataProvider<T> metadataProvider)
        {
            this.MetadataProvider = metadataProvider;
        }

        /// <inheritdoc/>
        public abstract bool TryExtract(T node, out LocalizableStringOccurence result);

        /// <summary>
        /// Creates a localized string.
        /// </summary>
        /// <param name="text">The localized text.</param>
        /// <param name="textPlural">The pluralization form for the localized text.</param>
        /// <param name="node">The node in which to get the localized string information.</param>
        protected LocalizableStringOccurence CreateLocalizedString(string text, string textPlural, T node)
        {
            var result = new LocalizableStringOccurence()
            {
                Text = text,
                TextPlural = textPlural,
                Location = this.MetadataProvider.GetLocation(node),
                Context = this.MetadataProvider.GetContext(node)
            };

            return result;
        }
    }
}
