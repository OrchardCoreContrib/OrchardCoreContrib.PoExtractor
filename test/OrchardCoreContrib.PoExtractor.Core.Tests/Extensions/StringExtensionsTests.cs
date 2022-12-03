using Xunit;

namespace OrchardCoreContrib.PoExtractor.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void TrimStart_TrimsTextFromStartOfString()
        {
            var text = "TEST-some-other-content-TEST";

            var result = text.TrimStart("TEST");

            Assert.Equal("-some-other-content-TEST", result);
        }
    }
}
