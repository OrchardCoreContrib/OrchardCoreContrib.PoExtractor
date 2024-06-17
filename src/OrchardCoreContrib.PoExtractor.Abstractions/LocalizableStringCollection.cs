using System;
using System.Collections.Generic;

namespace OrchardCoreContrib.PoExtractor;

/// <summary>
/// Represents collection of the all localizable strings in the project. Localizable strings with the same values are merged.
/// </summary>
public class LocalizableStringCollection
{
    private readonly Dictionary<string, LocalizableString> _values;

    /// <summary>
    /// Creates a new empty instance of the <see cref="LocalizableStringCollection" /> class.
    /// </summary>
    public LocalizableStringCollection()
    {
        _values = [];
    }

    /// <summary>
    /// Gets collection of all <see cref="LocalizableString"/> in the project.
    /// </summary>
    public IEnumerable<LocalizableString> Values => _values.Values;

    /// <summary>
    /// Adds <see cref="LocalizableStringOccurence"/> to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(LocalizableStringOccurence item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var key = item.Context + item.Text;
        if (_values.TryGetValue(key, out var localizedString))
        {
            localizedString.Locations.Add(item.Location);
        }
        else
        {
            _values.Add(key, new LocalizableString(item));
        }
    }

    /// <summary>
    /// Clear collection
    /// </summary>
    public void Clear() => _values.Clear();
}
