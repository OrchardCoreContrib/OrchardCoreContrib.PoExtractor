using PoExtractor.Core.Tests.Fakes;
using System.Linq;
using Xunit;

namespace PoExtractor.Core.Tests
{
    public class ErrorMessageAnnotationStringExtractorTests
    {
        [Fact]
        public void ExtractLocalizedStringsFromDataAnnotations()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values.Select(s => s.Text).ToList();

            Assert.NotEmpty(localizedStrings);
            Assert.Equal(6, localizedStrings.Count());
            Assert.Contains(localizedStrings, s => s == "The username is required.");
        }

        [Fact]
        public void DataAnnotationsExtractorShouldRespectErrorMessageOrder()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizableStringCollection = new LocalizableStringCollection();

            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

            // Assert
            var localizedStrings = localizableStringCollection.Values.Select(s => s.Text).ToList();

            Assert.Contains(localizedStrings, s => s == "Age should be in the range [15-45].");
        }
    }
}
