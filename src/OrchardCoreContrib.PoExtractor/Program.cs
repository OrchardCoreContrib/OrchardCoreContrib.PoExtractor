using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.DotNet.CS;
using OrchardCoreContrib.PoExtractor.DotNet.VB;
using OrchardCoreContrib.PoExtractor.Liquid;
using OrchardCoreContrib.PoExtractor.Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor
{
    public class Program
    {
        private static readonly string _defaultLanguage = Language.CSharp;
        private static readonly string _defaultTemplateEngine = TemplateEngine.Both;
        private static readonly string[] _ignoredProjects = new string[]
        {
            "docs",
            "src\\OrchardCore.Cms.Web",
            "src\\OrchardCore.Mvc.Web",
            "src\\Templates",
            "test"
        };

        public static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 6 || args.Length % 2 == 1)
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

            (string language, string templateEngine) = GetCliOptions(args);

            if (language == null || templateEngine == null)
            {
                ShowHelp();

                return;
            }

            var projectFiles = new List<string>();
            var projectProcessors = new List<IProjectProcessor>();

            if (language == Language.CSharp)
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

            if (templateEngine == TemplateEngine.Both)
            {
                projectProcessors.Add(new RazorProjectProcessor());
                projectProcessors.Add(new LiquidProjectProcessor());
            }
            else if (templateEngine == TemplateEngine.Razor)
            {
                projectProcessors.Add(new RazorProjectProcessor());
            }
            else if (templateEngine == TemplateEngine.Liquid)
            {
                projectProcessors.Add(new LiquidProjectProcessor());
            }

            foreach (var projectFile in projectFiles)
            {
                var projectPath = Path.GetDirectoryName(projectFile);
                var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var projectRelativePath = projectPath.Substring(projectBasePath.Length);

                if (IgnoredProject.ToList().Any(p => projectRelativePath.StartsWith(p)))
                {
                    continue;
                }

                var localizableStrings = new LocalizableStringCollection();
                foreach (var projectProcessor in projectProcessors)
                {
                    projectProcessor.Process(projectPath, projectBasePath, localizableStrings);
                }

                if (localizableStrings.Values.Any())
                {
                    var potPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(projectFile) + PoWriter.PortaleObjectTemplateExtension);

                    Directory.CreateDirectory(Path.GetDirectoryName(potPath));

                    using (var potFile = new PoWriter(potPath))
                    {
                        potFile.WriteRecord(localizableStrings.Values);
                    }
                }

                Console.WriteLine($"{Path.GetFileName(projectPath)}: Found {localizableStrings.Values.Count()} strings.");
            }
        }

        private static (string language, string templateEngine) GetCliOptions(string[] args)
        {
            var language = _defaultLanguage;
            var templateEngine = _defaultTemplateEngine;
            for (int i = 4; i <= args.Length; i += 2)
            {
                switch (args[i - 2])
                {
                    case "-l":
                    case "--language":
                        if (args[i - 1].Equals(Language.CSharp, StringComparison.CurrentCultureIgnoreCase))
                        {
                            language = Language.CSharp;
                        }
                        else if (args[i - 1].Equals(Language.VisualBasic, StringComparison.CurrentCultureIgnoreCase))
                        {
                            language = Language.VisualBasic;
                        }
                        else
                        {
                            language = null;
                        }

                        break;
                    case "-t":
                    case "--template":
                        if (args[i - 1].Equals(TemplateEngine.Razor, StringComparison.CurrentCultureIgnoreCase))
                        {
                            templateEngine = TemplateEngine.Razor;
                        }
                        else if (args[i - 1].Equals(TemplateEngine.Liquid, StringComparison.CurrentCultureIgnoreCase))
                        {
                            templateEngine = TemplateEngine.Liquid;
                        }
                        else
                        {
                            templateEngine = null;
                        }

                        break;
                    default:
                        language = null;
                        templateEngine = null;
                        break;
                }
            }

            return (language, templateEngine);
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
            Console.WriteLine("  -l, --language <C#|VB>             Specifies the code language to extracts translatable strings from.");
            Console.WriteLine("                                     Default: C# language");
            Console.WriteLine("  -t, --template <Razor|Liquid>      Specifies the template engine to extract the translatable strings from.");
            Console.WriteLine("                                     Default: Razor & Liquid templates");
        }
    }
}
