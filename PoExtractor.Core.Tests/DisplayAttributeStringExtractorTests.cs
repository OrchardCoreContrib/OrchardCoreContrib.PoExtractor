using PoExtractor.Core.Tests.Fakes;
using System.Linq;
using Xunit;

namespace PoExtractor.Core.Tests
{
    public class DisplayAttributeStringExtractorTests
    {
        [Fact]
        public void ExtractLocalizedNameFromDisplayAttribute()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values.Select(s => s.Text).ToList();

            Assert.Contains(localizedStrings, s => s == "First name");
        }

        [Fact]
        public void ExtractLocalizedShortNameFromDisplayAttribute()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values.Select(s => s.Text).ToList();

            Assert.Contains(localizedStrings, s => s == "1st name");
        }

        [Fact]
        public void ExtractLocalizedGroupNameFromDisplayAttribute()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values.Select(s => s.Text).ToList();

            Assert.Contains(localizedStrings, s => s == "Person info");
        }

        [Fact]
        public void ExtractLocalizedDescriptionFromDisplayAttribute()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values.Select(s => s.Text).ToList();

            Assert.Contains(localizedStrings, s => s == "The first name of the person");
        }
    }
}
