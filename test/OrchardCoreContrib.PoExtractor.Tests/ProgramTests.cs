using Xunit;

namespace OrchardCoreContrib.PoExtractor.Tests;

public class ProgramTests
{
    [Fact]
    public void Main_NoTemplateOption_UsesBothRazorAndLiquid()
    {
        // Arrange
        var root = Path.Combine(Path.GetTempPath(), "PoExtractorTests", Guid.NewGuid().ToString("N"));
        var input = Path.Combine(root, "input");
        var output = Path.Combine(root, "output");
        var project = Path.Combine(input, "TestModule");

        Directory.CreateDirectory(project);
        Directory.CreateDirectory(output);

        File.WriteAllText(Path.Combine(project, "TestModule.csproj"), """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
""");

        File.WriteAllText(Path.Combine(project, "Index.cshtml"), """
@T["Hello from Razor"]
""");

        File.WriteAllText(Path.Combine(project, "index.liquid"), """
{{ "Hello from Liquid" | t }}
""");

        var potFileName = "all.pot";

        try
        {
            // Act
            Program.Main(
            [
                input,
                output,
                "--single", potFileName
            ]);

            // Assert
            var potPath = Path.Combine(output, potFileName);
            Assert.True(File.Exists(potPath));

            var pot = File.ReadAllText(potPath);
            Assert.Contains("Hello from Razor", pot);
            Assert.Contains("Hello from Liquid", pot);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [Fact]
    public void Main_LiquidProcessorConfigurationOption_LoadsLiquidTagsFromJsonFile()
    {
        // Arrange
        var root = Path.Combine(Path.GetTempPath(), "PoExtractorTests", Guid.NewGuid().ToString("N"));
        var input = Path.Combine(root, "input");
        var output = Path.Combine(root, "output");
        var project = Path.Combine(input, "TestModule");
        var configurationPath = Path.Combine(root, "liquid-config.json");

        Directory.CreateDirectory(project);
        Directory.CreateDirectory(output);

        File.WriteAllText(Path.Combine(project, "TestModule.csproj"), """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
""");

        File.WriteAllText(Path.Combine(project, "index.liquid"), """
{% scriptblock at:"FootScript" %}
{{ "Hello from Liquid configured tag" | t }}
{% endscriptblock %}
""");

        File.WriteAllText(configurationPath, """
{
  "InlineTags": [],
  "BlockTags": ["scriptblock"]
}
""");

        var potFileName = "liquid-config.pot";

        try
        {
            // Act
            Program.Main(
            [
                input,
                output,
                "--template", "Liquid",
                "--liquid-processor-configuration", configurationPath,
                "--single", potFileName
            ]);

            // Assert
            var potPath = Path.Combine(output, potFileName);
            Assert.True(File.Exists(potPath));

            var pot = File.ReadAllText(potPath);
            Assert.Contains("Hello from Liquid configured tag", pot);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
}
