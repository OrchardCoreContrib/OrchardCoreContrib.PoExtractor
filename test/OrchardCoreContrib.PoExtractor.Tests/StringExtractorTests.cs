using OrchardCoreContrib.PoExtractor.DotNet.CS;
using Xunit;

namespace OrchardCoreContrib.PoExtractor.Tests
{
    public class StringExtractorTests
    {
        private readonly CSharpProjectProcessor _csharpProjectProcessor = new();

        [Fact]
        public void ExtractSingularText()
        {
            IEnumerable<string> extractedStrings = ExtractStrings();
            Assert.Contains(extractedStrings, s => s == "Thing");
        }

        [Fact]
        public void ExtractPluralText()
        {
            IEnumerable<string> extractedStrings = ExtractStrings();
            Assert.Contains(extractedStrings, s => s == "{0} thing");
            Assert.Contains(extractedStrings, s => s == "{0} things");
        }

        private IEnumerable<string> ExtractStrings()
        {
            var localizableStringCollection = new LocalizableStringCollection();

            _csharpProjectProcessor.Process("ProjectFiles", "ProjectFiles", localizableStringCollection);

            var localizedStrings = localizableStringCollection.Values
                .SelectMany(s => !string.IsNullOrWhiteSpace(s.TextPlural) ? new[] { s.Text, s.TextPlural } : new[] { s.Text })
                .ToList();

            return localizedStrings;
        }
    }
}
