namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Represents a class that contains a set of localizer identifier accessors.
/// </summary>
public static class LocalizerAccessors
{
    /// <summary>
    /// Gets the localizer identifier for IStringLocalizer or IHtmlStringLocalizer in views.
    /// </summary>
    public static readonly string DefaultLocalizerIdentifier = "T";

    /// <summary>
    /// Gets the localizer identifier for IStringLocalizer.
    /// </summary>
    public static readonly string StringLocalizerIdentifier = "S";

    /// <summary>
    /// Gets the localizer identifier for IHtmlStringLocalizer.
    /// </summary>
    public static readonly string HtmlLocalizerIdentifier = "H";

    /// <summary>
    /// Gets the localizer identifiers.
    /// </summary>
    public static string[] LocalizerIdentifiers =
    [
        DefaultLocalizerIdentifier,
        StringLocalizerIdentifier,
        HtmlLocalizerIdentifier,
    ];

    /// <summary>
    /// Determines whether the identifier matches a supported localizer accessor name.
    /// </summary>
    public static bool IsLocalizerIdentifier(string identifier) =>
        LocalizerIdentifiers.Contains(identifier, StringComparer.Ordinal);
}
