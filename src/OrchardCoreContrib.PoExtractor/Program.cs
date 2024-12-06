using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.DotNet.CS;
using OrchardCoreContrib.PoExtractor.DotNet.VB;
using OrchardCoreContrib.PoExtractor.Liquid;
using OrchardCoreContrib.PoExtractor.Razor;

namespace OrchardCoreContrib.PoExtractor;

public class Program
{
    private static readonly string _defaultLanguage = Language.CSharp;
    private static readonly string _defaultTemplateEngine = TemplateEngine.Both;

    public static void Main(string[] args)
    {
        if (args.Length < 2 || args.Length > 10 || args.Length % 2 == 1)
        {
            ShowHelp();

            return;
        }

        var inputPath = new DirectoryInfo(args[0]).FullName;
        var outputPath = new DirectoryInfo(args[1]).FullName;

        if (!Directory.Exists(inputPath))
        {
            ShowHelp();

            return;
        }

        var options = GetCliOptions(args);

        if (options.Language == null || options.TemplateEngine == null)
        {
            ShowHelp();

            return;
        }

        var projectFiles = new List<string>();
        var projectProcessors = new List<IProjectProcessor>();

        if (options.Language == Language.CSharp)
        {
            projectProcessors.Add(new CSharpProjectProcessor());

            projectFiles.AddRange(Directory
                .EnumerateFiles(inputPath, $"*{ProjectExtension.CS}", SearchOption.AllDirectories)
                .OrderBy(f => f));
        }
        else
        {
            projectProcessors.Add(new VisualBasicProjectProcessor());

            projectFiles.AddRange(Directory
                .EnumerateFiles(inputPath, $"*{ProjectExtension.VB}", SearchOption.AllDirectories)
                .OrderBy(f => f));
        }

        if (options.TemplateEngine == TemplateEngine.Both)
        {
            projectProcessors.Add(new RazorProjectProcessor());
            projectProcessors.Add(new LiquidProjectProcessor());
        }
        else if (options.TemplateEngine == TemplateEngine.Razor)
        {
            projectProcessors.Add(new RazorProjectProcessor());
        }
        else if (options.TemplateEngine == TemplateEngine.Liquid)
        {
            projectProcessors.Add(new LiquidProjectProcessor());
        }

        var isSingleFileOutput = !string.IsNullOrEmpty(options.SingleOutputFile);
        var localizableStrings = new LocalizableStringCollection();
        foreach (var projectFile in projectFiles)
        {
            var projectPath = Path.GetDirectoryName(projectFile);
            var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
            var projectRelativePath = projectPath[projectBasePath.Length..];
            var rootedProject = projectPath[(projectPath.IndexOf(inputPath) + inputPath.Length + 1)..];
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
                    var potPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(projectFile) + PoWriter.PortaleObjectTemplateExtension);

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
                var potPath = Path.Combine(outputPath, options.SingleOutputFile);

                Directory.CreateDirectory(Path.GetDirectoryName(potPath));

                using var potFile = new PoWriter(potPath);
                potFile.WriteRecord(localizableStrings.Values);
            }

            Console.WriteLine($"Found {localizableStrings.Values.Count()} strings.");
        }
    }

    private static GetCliOptionsResult GetCliOptions(string[] args)
    {
        var result = new GetCliOptionsResult
        {
            Language = _defaultLanguage,
            TemplateEngine = _defaultTemplateEngine,
            SingleOutputFile = null,
        };

        for (int i = 4; i <= args.Length; i += 2)
        {
            switch (args[i - 2])
            {
                case "-l":
                case "--language":
                    if (args[i - 1].Equals(Language.CSharp, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Language = Language.CSharp;
                    }
                    else if (args[i - 1].Equals(Language.VisualBasic, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Language = Language.VisualBasic;
                    }
                    else
                    {
                        result.Language = null;
                    }

                    break;
                case "-t":
                case "--template":
                    if (args[i - 1].Equals(TemplateEngine.Razor, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.TemplateEngine = TemplateEngine.Razor;
                    }
                    else if (args[i - 1].Equals(TemplateEngine.Liquid, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.TemplateEngine = TemplateEngine.Liquid;
                    }
                    else
                    {
                        result.TemplateEngine = null;
                    }

                    break;
                case "-i":
                case "--ignore":
                    if (!string.IsNullOrEmpty(args[i - 1]))
                    {
                        var ignoredProjects = args[i - 1].Split(',', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var ignoredProject in ignoredProjects)
                        {
                            IgnoredProject.Add(ignoredProject);
                        }
                    }

                    break;
                case "--localizer":
                    if (!string.IsNullOrEmpty(args[i - 1]))
                    {
                        var localizerIdentifiers = args[i - 1].Split(',', StringSplitOptions.RemoveEmptyEntries);

                        LocalizerAccessors.LocalizerIdentifiers = localizerIdentifiers;
                    }

                    break;
                case "-s":
                case "--single":
                    if (!string.IsNullOrEmpty(args[i - 1]))
                    {
                        result.SingleOutputFile = args[i - 1];
                    }

                    break;
                case "-p":
                case "--plugin":
                    if (File.Exists(args[i - 1]))
                    {
                        result.Plugins.Add(args[i - 1]);
                    }

                    break;
                default:
                    result.Language = null;
                    result.TemplateEngine = null;
                    break;
            }
        }

        return result;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  extractpo <INPUT_PATH> <OUTPUT_PATH> [options]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  <INPUT_PATH> The path to the input directory, all projects at the the path will be processed.");
        Console.WriteLine("  <OUTPUT_PATH> The path to a directory where POT files will be generated.");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -l, --language <C#|VB>                 Specifies the code language to extracts translatable strings from.");
        Console.WriteLine("                                         Default: C# language");
        Console.WriteLine("  -t, --template <Razor|Liquid>          Specifies the template engine to extract the translatable strings from.");
        Console.WriteLine("                                         Default: Razor & Liquid templates");
        Console.WriteLine("  -i, --ignore project1,project2         Ignores extracting PO filed from a given project(s).");
        Console.WriteLine("  --localizer localizer1,localizer2      Specifies the name of the localizer(s) that will be used during the extraction process.");
        Console.WriteLine("  -s, --single <FILE_NAME>               Specifies the single output file.");
        Console.WriteLine("  -p, --plugin <FILE_NAME>               A path to a C# script file which can define further IProjectProcessor");
        Console.WriteLine("                                         implementations. You can have multiple of this switch in a call.");
    }
}
