namespace OrchardCoreContrib.PoExtractor.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("TEST-some-other-content-TEST", "TEST", "-some-other-content-TEST")]
    [InlineData(
        @"D:\Repositories\OrchardCoreContrib.PoExtractor\src\WebApplication1\WebApplication1\Pages\Index.cshtml",
        @"D:\Repositories\OrchardCoreContrib.PoExtractor\src\WebApplication1\",
        "WebApplication1\\Pages\\Index.cshtml")]
    public void TrimStart_TrimsTextFromStartOfString(string text, string textToBeRemoved, string expected)
    {
        // Act
        var result = text.TrimStart(textToBeRemoved);

        // Assert
        Assert.Equal(expected, result);
    }
}
