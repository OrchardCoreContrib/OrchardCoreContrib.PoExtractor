namespace OrchardCoreContrib.PoExtractor
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes the given value from the start of the text.
        /// </summary>
        /// <param name="text">The </param>
        /// <param name="value">The value to be removed.</param>
        public static string TrimStart(this string text, string value)
        {
            var index = text.IndexOf(value);

            return index < 0
                ? text
                : text.Remove(index, value.Length);
        }
    }
}
