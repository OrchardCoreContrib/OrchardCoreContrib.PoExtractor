using PoExtractor.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace PoExtractor.Core.Tests {
    public class StringExtensionsTests {
        [Fact]
        public void TrimStart_TrimsTextFromStartOfString() {
            var text = "TEST-some-other-content-TEST";

            var result = text.TrimStart("TEST");

            Assert.Equal("-some-other-content-TEST", result);
        }
    }
}
