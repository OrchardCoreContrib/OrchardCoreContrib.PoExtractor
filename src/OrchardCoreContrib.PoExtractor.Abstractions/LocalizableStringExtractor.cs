using System;

namespace OrchardCoreContrib.PoExtractor
{
    /// <summary>
    /// Represents a base class for extracting a localizable strings.
    /// </summary>
    /// <typeparam name="TNode">The type of the node.</typeparam>
    public abstract class LocalizableStringExtractor<TNode> : IStringExtractor<TNode>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="LocalizableStringExtractor{T}"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
        public LocalizableStringExtractor(IMetadataProvider<TNode> metadataProvider)
        {
            MetadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
        }

        protected IMetadataProvider<TNode> MetadataProvider { get; }

        /// <inheritdoc/>
        public abstract bool TryExtract(TNode node, out LocalizableStringOccurence result);

        /// <summary>
        /// Creates a localized string.
        /// </summary>
        /// <param name="text">The localized text.</param>
        /// <param name="textPlural">The pluralization form for the localized text.</param>
        /// <param name="node">The node in which to get the localized string information.</param>
        protected LocalizableStringOccurence CreateLocalizedString(string text, string textPlural, TNode node)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            var result = new LocalizableStringOccurence
            {
                Text = text,
                TextPlural = textPlural,
                Location = MetadataProvider.GetLocation(node),
                Context = MetadataProvider.GetContext(node)
            };

            return result;
        }
    }
}
