namespace PoExtractor.Core {
    public static class StringExtransions {
        public static string TrimStart(this string text, string value) {
            return text.StartsWith(value) ? text.Remove(0, value.Length) : text;
        }
    }
}
