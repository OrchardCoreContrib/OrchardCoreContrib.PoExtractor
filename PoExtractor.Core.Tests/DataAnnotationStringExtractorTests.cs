using PoExtractor.Core.Tests.Fakes;
using System.Linq;
using Xunit;

namespace PoExtractor.Core.Tests
{
    public class DataAnnotationStringExtractorTests
    {
        [Fact]
        public void ExtractlocalizedStringsFromDataAnnotations()
        {
            // Arrange
            var csProjectProcessor = new FakeCSharpProjectProcessor();
            var localizedStrings = new LocalizableStringCollection();
            
            // Act
            csProjectProcessor.Process(string.Empty, string.Empty, localizedStrings);

            // Assert
            Assert.NotEmpty(localizedStrings.Values);
            Assert.Equal("The username is required.", localizedStrings.Values.First().Text);
        }
    }
}
