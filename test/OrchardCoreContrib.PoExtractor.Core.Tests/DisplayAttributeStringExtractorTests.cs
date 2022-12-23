using OrchardCoreContrib.PoExtractor.Tests.Fakes;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.Tests
{
    public class DisplayAttributeStringExtractorTests
    {
        private readonly FakeCSharpProjectProcessor _fakeCSharpProjectProcessor = new();

        [Fact]
        public void ExtractLocalizedNameFromDisplayAttribute()
        {
            // Arrange
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            _fakeCSharpProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values
                .Select(s => s.Text)
                .ToList();

            Assert.Contains(localizedStrings, s => s == "First name");
        }

        [Fact]
        public void ExtractLocalizedShortNameFromDisplayAttribute()
        {
            // Arrange
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            _fakeCSharpProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values
                .Select(s => s.Text)
                .ToList();

            Assert.Contains(localizedStrings, s => s == "1st name");
        }

        [Fact]
        public void ExtractLocalizedGroupNameFromDisplayAttribute()
        {
            // Arrange
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            _fakeCSharpProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values
                .Select(s => s.Text)
                .ToList();

            Assert.Contains(localizedStrings, s => s == "Person info");
        }

        [Fact]
        public void ExtractLocalizedDescriptionFromDisplayAttribute()
        {
            // Arrange
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            _fakeCSharpProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values
                .Select(s => s.Text)
                .ToList();

            Assert.Contains(localizedStrings, s => s == "The first name of the person");
        }
    }
}
