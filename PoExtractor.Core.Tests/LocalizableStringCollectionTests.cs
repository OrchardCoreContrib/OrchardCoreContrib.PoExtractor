using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PoExtractor.Core.Tests {
    public class LocalizableStringCollectionTests {
        private LocalizableStringOccurence s1 = new LocalizableStringOccurence() { Text = "Computer", Location = new LocalizableStringLocation() { SourceFileLine = 1 } };
        private LocalizableStringOccurence s2 = new LocalizableStringOccurence() { Text = "Computer", Location = new LocalizableStringLocation() { SourceFileLine = 1 } };
        private LocalizableStringOccurence otherS = new LocalizableStringOccurence() { Text = "Keyboard", Location = new LocalizableStringLocation() { SourceFileLine = 1 } };

        [Fact]
        public void WhenCreated_ValuesCollectionIsEmpty() {
            var sut = new LocalizableStringCollection();

            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Add_CreatesNewLocalizableString_IfTheCollectionIsEmpty() {
            var sut = new LocalizableStringCollection();

            sut.Add(s1);

            Assert.Single(sut.Values);

            var result = sut.Values.Single();
            Assert.Equal(s1.Text, result.Text);
            Assert.Contains(s1.Location, result.Locations);
        }

        [Fact]
        public void Add_AddsLocationToLocalizableString_IfTheCollectionContainsSameString() {
            var sut = new LocalizableStringCollection();

            sut.Add(s1);
            sut.Add(s2);

            Assert.Single(sut.Values);

            var result = sut.Values.Single();
            Assert.Equal(s1.Text, result.Text);
            Assert.Contains(s1.Location, result.Locations);
            Assert.Contains(s2.Location, result.Locations);
        }

        [Fact]
        public void Add_CreatesNewLocalizableString_IfTheCollectionDoesntContainSameString() {
            var sut = new LocalizableStringCollection();

            sut.Add(s1);
            sut.Add(otherS);

            Assert.Contains(sut.Values, o => o.Text == s1.Text);
            Assert.Contains(sut.Values, o => o.Text == s2.Text);
        }
    }
}
