namespace OrchardCoreContrib.PoExtractor.Tests
{
    public class LocalizableStringTests
    {
        [Fact]
        public void Constructor_PopulatesProperties()
        {
            // Arrange
            var source = new LocalizableStringOccurence()
            {
                Context = "OrchardCoreContrib.PoExtractor",
                Text = "Computer",
                TextPlural = "Computers",
                Location = new LocalizableStringLocation()
                {
                    Comment = "Comment",
                    SourceFile = "Test.cs",
                    SourceFileLine = 1
                }
            };

            // Act
            var result = new LocalizableString(source);

            // Assert
            Assert.Equal(source.Context, result.Context);
            Assert.Equal(source.Text, result.Text);
            Assert.Equal(source.TextPlural, result.TextPlural);
            Assert.Single(result.Locations, source.Location);
        }
    }
}
