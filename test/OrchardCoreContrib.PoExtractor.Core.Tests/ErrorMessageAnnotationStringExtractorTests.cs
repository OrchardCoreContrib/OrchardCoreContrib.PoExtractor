using OrchardCoreContrib.PoExtractor.Tests.Fakes;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.Tests;

public class ErrorMessageAnnotationStringExtractorTests
{
    private readonly FakeCSharpProjectProcessor _fakeCSharpProjectProcessor = new();

    [Fact]
    public void ExtractLocalizedStringsFromDataAnnotations()
    {
        // Arrange
        var localizableStringCollection = new LocalizableStringCollection();

        // Act
        _fakeCSharpProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

        // Assert
        var localizedStrings = localizableStringCollection.Values
            .Select(s => s.Text)
            .ToList();

        Assert.NotEmpty(localizedStrings);
        Assert.Equal(6, localizedStrings.Count);
        Assert.Contains(localizedStrings, s => s == "The username is required.");
    }

    [Fact]
    public void DataAnnotationsExtractorShouldRespectErrorMessageOrder()
    {
        // Arrange
        var localizableStringCollection = new LocalizableStringCollection();

        // Act
        _fakeCSharpProjectProcessor.Process(string.Empty, string.Empty, localizableStringCollection);

        // Assert
        var localizedStrings = localizableStringCollection.Values
            .Select(s => s.Text)
            .ToList();

        Assert.Contains(localizedStrings, s => s == "Age should be in the range [15-45].");
    }
}
