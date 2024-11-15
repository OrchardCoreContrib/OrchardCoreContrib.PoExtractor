using System;

namespace OrchardCoreContrib.PoExtractor;

/// <summary>
/// Represents a base class for extracting a localizable strings.
/// </summary>
/// <typeparam name="TNode">The type of the node.</typeparam>
/// <remarks>
/// Creates a new instance of a <see cref="LocalizableStringExtractor{T}"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
public abstract class LocalizableStringExtractor<TNode>(IMetadataProvider<TNode> metadataProvider) : IStringExtractor<TNode>
{
    protected IMetadataProvider<TNode> MetadataProvider { get; } = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));

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
            Location = metadataProvider.GetLocation(node),
            Context = metadataProvider.GetContext(node)
        };

        return result;
    }
}
