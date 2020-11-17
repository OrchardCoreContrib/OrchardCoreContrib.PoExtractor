using PoExtractor.Core;
using System;
using System.Linq;
using Xunit;

namespace PoExtractor.Liquid.Tests
{
    public class LiquidProjectProcessorTests
    {
        readonly LiquidProjectProcessor processor = new();
        readonly LocalizableStringCollection strings = new ();

        [Fact]
        public void ExtractsStringFromLiquidProperty()
        {
            processor.Process("ProjectFiles", string.Empty, strings);

            Assert.Contains(strings.Values, s => s.Text == "string in variable");
        }
        
        [Fact]
        public void ExtractsStringFromLiquidExpression()
        {
            processor.Process("ProjectFiles", string.Empty, strings);

            Assert.Contains(strings.Values, s => s.Text == "string in expression");
        }
    }
}
