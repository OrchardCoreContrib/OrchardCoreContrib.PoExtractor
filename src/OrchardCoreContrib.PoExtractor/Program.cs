using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using OrchardCore.Modules;
using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.DotNet.CS;
using OrchardCoreContrib.PoExtractor.DotNet.VB;
using OrchardCoreContrib.PoExtractor.Liquid;
using OrchardCoreContrib.PoExtractor.Razor;

namespace OrchardCoreContrib.PoExtractor;

public class Program
{
    public static void Main(string[] args)
    {
        var app = new CommandLineApplication();

        app.HelpOption();

        // Arguments
        var inputPath = app.Argument("Input Path", "The path to the input directory, all projects at the the path will be processed.")
            .IsRequired();
        var outputPath = app.Argument("Output Path", "The path to a directory where POT files will be generated.")
            .IsRequired();

        // Options
        var language = app.Option("-l|--language <LANGUAGE>", "Specifies the code language to extracts translatable strings from.", CommandOptionType.SingleValue, option =>
        {
            option.Accepts(cfg => cfg.Values("C#", "VB"));
            option.DefaultValue = "C#";
        });
        var template = app.Option("-t|--template <TEMPLATE>", "Specifies the template engine to extract the translatable strings from.", CommandOptionType.SingleValue, option =>
            option.Accepts(cfg => cfg.Values("Razor", "Liquid"))
        );
        var ignoredProjects = app.Option("-i|--ignore <IGNORED_PROJECTS>", "Ignores extracting PO files from a given project(s).", CommandOptionType.MultipleValue);
        var localizers = app.Option("--localizer <LOCALIZERS>", "Specifies the name of the localizer(s) that will be used during the extraction process.", CommandOptionType.MultipleValue);
        var single = app.Option("-s|--single <FILE_NAME>", "Specifies the single output file.", CommandOptionType.SingleValue);
        var plugins = app.Option(
            "-p|--plugin <FILE_NAME_OR_HTTPS_URL>",
            "A path or web URL with HTTPS scheme to a C# script (.csx) file which can define further " +
            "IProjectProcessor implementations. You can have multiple of this switch in a call.",
            CommandOptionType.MultipleValue,
            option => option.OnValidate(_ => option
                .Values
                .All(item => File.Exists(item) || item.StartsWithOrdinalIgnoreCase("https://"))
                ? ValidationResult.Success
                : new ValidationResult("Plugin must be an existing local file or a valid HTTPS URL.")));

        app.OnExecuteAsync(async cancellationToken =>
        {
            if (!Directory.Exists(inputPath.Value))
            {
                Console.WriteLine($"The input path '{inputPath.Value}' does not exist.");

                return;
            }

            foreach (var ignoredProject in ignoredProjects.Values)
            {
                IgnoredProject.Add(ignoredProject);
            }

            LocalizerAccessors.LocalizerIdentifiers = [.. localizers.Values];

            var projectFiles = new List<string>();
            var projectProcessors = new List<IProjectProcessor>();

            if (language.Value() == Language.CSharp)
            {
                projectProcessors.Add(new CSharpProjectProcessor());

                projectFiles.AddRange(Directory
                    .EnumerateFiles(inputPath.Value, $"*{ProjectExtension.CS}", SearchOption.AllDirectories)
                    .OrderBy(f => f));
            }
            else
            {
                projectProcessors.Add(new VisualBasicProjectProcessor());

                projectFiles.AddRange(Directory
                    .EnumerateFiles(inputPath.Value, $"*{ProjectExtension.VB}", SearchOption.AllDirectories)
                    .OrderBy(f => f));
            }

            if (template.Value() == TemplateEngine.Both)
            {
                projectProcessors.Add(new RazorProjectProcessor());
                projectProcessors.Add(new LiquidProjectProcessor());
            }
            else if (template.Value() == TemplateEngine.Razor)
            {
                projectProcessors.Add(new RazorProjectProcessor());
            }
            else if (template.Value() == TemplateEngine.Liquid)
            {
                projectProcessors.Add(new LiquidProjectProcessor());
            }

            if (plugins.Values.Count > 0)
            {
                await PluginHelper.ProcessPluginsAsync(plugins.Values, projectProcessors, projectFiles);
            }

            var isSingleFileOutput = !string.IsNullOrEmpty(single.Value());
            var localizableStrings = new LocalizableStringCollection();
            foreach (var projectFile in projectFiles)
            {
                var projectPath = Path.GetDirectoryName(projectFile);
                var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var projectRelativePath = projectPath[projectBasePath.Length..];
                var rootedProject = projectPath == inputPath.Value
                    ? projectPath 
                    : projectPath[(projectPath.IndexOf(inputPath.Value, StringComparison.Ordinal) + inputPath.Value.Length + 1)..];
                if (IgnoredProject.ToList().Any(p => rootedProject.StartsWith(p)))
                {
                    continue;
                }

                foreach (var projectProcessor in projectProcessors)
                {
                    projectProcessor.Process(projectPath, projectBasePath, localizableStrings);
                }

                if (!isSingleFileOutput)
                {
                    if (localizableStrings.Values.Any())
                    {
                        var potPath = Path.Combine(outputPath.Value, Path.GetFileNameWithoutExtension(projectFile) + PoWriter.PortaleObjectTemplateExtension);

                        Directory.CreateDirectory(Path.GetDirectoryName(potPath));

                        using var potFile = new PoWriter(potPath);
                        potFile.WriteRecord(localizableStrings.Values);
                    }

                    Console.WriteLine($"{Path.GetFileName(projectPath)}: Found {localizableStrings.Values.Count()} strings.");
                    localizableStrings.Clear();
                }
            }

            if (isSingleFileOutput)
            {
                if (localizableStrings.Values.Any())
                {
                    var potPath = Path.Combine(outputPath.Value, single.Value());

                    Directory.CreateDirectory(Path.GetDirectoryName(potPath));

                    using var potFile = new PoWriter(potPath);
                    potFile.WriteRecord(localizableStrings.Values);
                }

                Console.WriteLine($"Found {localizableStrings.Values.Count()} strings.");
            }
        });

        app.Execute(args);
    }
}
