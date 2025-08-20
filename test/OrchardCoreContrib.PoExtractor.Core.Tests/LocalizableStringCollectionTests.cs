namespace OrchardCoreContrib.PoExtractor.Tests;

public class LocalizableStringCollectionTests
{
    private readonly LocalizableStringOccurence _localizedString1 = new()
    {
        Text = "Computer",
        Location = new LocalizableStringLocation() { SourceFileLine = 1 }
    };
    private readonly LocalizableStringOccurence _localizedString2 = new()
    {
        Text = "Computer",
        Location = new LocalizableStringLocation() { SourceFileLine = 1 }
    };
    private readonly LocalizableStringOccurence _localizedString3 = new()
    {
        Text = "Keyboard", Location = new LocalizableStringLocation() { SourceFileLine = 1 }
    };

    [Fact]
    public void WhenCreated_ValuesCollectionIsEmpty()
    {
        // Arrange
        var localizableString = new LocalizableStringCollection();

        // Assert
        Assert.Empty(localizableString.Values);
    }

    [Fact]
    public void Add_CreatesNewLocalizableString_IfTheCollectionIsEmpty()
    {
        // Arrange
        var localizableString = new LocalizableStringCollection();

        // Act
        localizableString.Add(_localizedString1);

        // Assert
        var result = Assert.Single(localizableString.Values);
        Assert.Equal(_localizedString1.Text, result.Text);
        Assert.Contains(_localizedString1.Location, result.Locations);
    }

    [Fact]
    public void Add_AddsLocationToLocalizableString_IfTheCollectionContainsSameString()
    {
        // Arrange
        var localizableString = new LocalizableStringCollection();

        // Act
        localizableString.Add(_localizedString1);
        localizableString.Add(_localizedString2);

        // Assert
        var result = Assert.Single(localizableString.Values);
        Assert.Equal(_localizedString1.Text, result.Text);
        Assert.Contains(_localizedString1.Location, result.Locations);
        Assert.Contains(_localizedString2.Location, result.Locations);
    }

    [Fact]
    public void Add_CreatesNewLocalizableString_IfTheCollectionDoesntContainSameString()
    {
        // Arrange
        var localizableString = new LocalizableStringCollection();

        // Act
        localizableString.Add(_localizedString1);
        localizableString.Add(_localizedString3);

        // Assert
        Assert.Contains(localizableString.Values, o => o.Text == _localizedString1.Text);
        Assert.Contains(localizableString.Values, o => o.Text == _localizedString2.Text);
    }
}
