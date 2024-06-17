using Microsoft.Extensions.Localization;
using NSubstitute;
using Xunit;

namespace OrchardCoreContrib.PoExtractor.Tests;

public class ExecutionTests
{
    private readonly IStringLocalizer<ExecutionTests> _localizer = Substitute.For<IStringLocalizer<ExecutionTests>>();

    public ExecutionTests()
    {
        var _ = _localizer["fake_one"];
    }

    [Fact]
    public void Should_get_translations()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "test_should_get_translations");
        var args = new string[]{
            Directory.GetCurrentDirectory().Split("/OrchardCoreContrib.PoExtractor.Tests").First(), // first element: input project path
            outputPath,
            "--localizer",
            "_localizer"
        };

        Program.Main(args);

        var files = Directory.EnumerateFiles(outputPath)
            .Where(f => f.Contains("OrchardCoreContrib.PoExtractor.Tests"));

        Assert.Single(files);

        using var reader = new StreamReader(files.Single());
        string potContent = reader.ReadToEnd();

        Assert.Contains("msgid \"fake_one\"", potContent);
        Assert.Contains("msgctxt \"OrchardCoreContrib.PoExtractor.Tests.ExecutionTests\"", potContent);
    }
}