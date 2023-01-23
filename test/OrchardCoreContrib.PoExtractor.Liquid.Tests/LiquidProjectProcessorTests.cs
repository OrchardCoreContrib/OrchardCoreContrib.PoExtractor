using Xunit;

namespace OrchardCoreContrib.PoExtractor.Liquid.Tests
{
    public class LiquidProjectProcessorTests
    {
        private readonly LiquidProjectProcessor _processor = new();
        private readonly LocalizableStringCollection _localizableStrings = new();

        [Fact]
        public void ExtractsStringFromLiquidProperty()
        {
            // Act
            _processor.Process("ProjectFiles", "DummyBasePath", _localizableStrings);

            // Assert
            Assert.Contains(_localizableStrings.Values, s => s.Text == "string in variable");
        }

        [Fact]
        public void ExtractsStringFromLiquidExpression()
        {
            // Act
            _processor.Process("ProjectFiles", "DummyBasePath", _localizableStrings);

            // Assert
            Assert.Contains(_localizableStrings.Values, s => s.Text == "string in expression");
        }
    }
}
