using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PoExtractor.Core.Tests {
    public class LocalizableStringTests {
        [Fact]
        public void Constructor_PopulatesProperties() {
            var source = new LocalizableStringOccurence() {
                Context = "PoExtractor",
                Text = "Computer",
                TextPlural = "Computers",
                Location = new LocalizableStringLocation() {
                    Comment = "Comment",
                    SourceFile = "Test.cs",
                    SourceFileLine = 1
                }
            };

            var result = new LocalizableString(source);

            Assert.Equal(source.Context, result.Context);
            Assert.Equal(source.Text, result.Text);
            Assert.Equal(source.TextPlural, result.TextPlural);

            Assert.Single(result.Locations, source.Location);
        }
    }
}
