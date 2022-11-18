namespace OrchardCoreContrib.PoExtractor
{
    public static class StringExtensions
    {
        public static string TrimStart(this string text, string value)
        {
            return text.StartsWith(value) ? text.Remove(0, value.Length) : text;
        }
    }
}
