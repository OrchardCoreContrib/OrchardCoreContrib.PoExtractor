using System;

namespace OrchardCoreContrib.PoExtractor;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Removes the given value from the start of the text.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="trimText">The value to be trimmed.</param>
    public static string TrimStart(this string text, string trimText)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
        }

        if (string.IsNullOrEmpty(trimText))
        {
            throw new ArgumentException($"'{nameof(trimText)}' cannot be null or empty.", nameof(trimText));
        }

        var index = text.IndexOf(trimText);

        return index < 0
            ? text
            : text.Remove(index, trimText.Length);
    }
}
