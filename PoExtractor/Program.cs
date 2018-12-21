using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using PoExtractor.Core.Extractors;
using PoExtractor.Core.MetadataProviders;

namespace PoExtractor.Core {
    class Program {
        private static Func<string, string, string, string> DefaultOutputPathGenerator = (string basePath, string outputBasePath, string projectFilePath) => {
            var projectPath = Path.GetDirectoryName(projectFilePath);
            return Path.Combine(outputBasePath, projectPath.TrimStart(basePath + Path.DirectorySeparatorChar) + ".pot");
        };

        private static Func<string, string, string, string> OrchardOutputPathGenerator = (string basePath, string outputBasePath, string projectFilePath) => {
            var projectRelativePath = projectFilePath.TrimStart(Path.Combine(basePath, "src" + Path.DirectorySeparatorChar));
            var projectDir = projectRelativePath.Split(Path.DirectorySeparatorChar).First();

            var projectPath = Path.GetDirectoryName(projectFilePath);


            return Path.Combine(outputBasePath, projectDir, Path.GetFileNameWithoutExtension(projectFilePath) + ".pot");
        };

        private static string[] ProjectBlacklist = new[] { "test", "src\\OrchardCore.Cms.Web", "src\\OrchardCore.Mvc.Web", "src\\OrchardCore.Nancy.Web" };

        static void Main(string[] args) {
            if (args.Length != 2) {
                WriteHelp();
                return;
            }

            var basePath = args[0];
            var outputBasePath = args[1];

            string[] projectFiles;
            if (Directory.Exists(basePath)) {
                projectFiles = Directory.EnumerateFiles(basePath, "*.csproj", SearchOption.AllDirectories).ToArray();
            } else {
                WriteHelp();
                return;
            }

            foreach (var projectFilePath in projectFiles) {
                var projectPath = Path.GetDirectoryName(projectFilePath);
                var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var projectRelativePath = projectPath.TrimStart(basePath + Path.DirectorySeparatorChar);

                if (ProjectBlacklist.Any(o => projectRelativePath.StartsWith(o))) {
                    continue;
                }

                ProcessProject(projectPath, projectBasePath, OrchardOutputPathGenerator(args[0], args[1], projectFilePath));
            }
        }

        private static void WriteHelp() {
            Console.WriteLine("Usage: PoExtractor.exe input output");
            Console.WriteLine("    input: Path to input directory, all projects at the the path will be processed");
            Console.WriteLine("    output: Path to a directory where POT files will be generated");
        }

        private static void ProcessProject(string projectPath, string projectBasePath, string outputFilePath) {
            var codeMetadataProvider = new CodeMetadataProvider(projectBasePath);
            var localizedStringsCollector = new LocalizableStringCollector(
                new ILocalizableStringExtractor[] {
                    new SingularStringExtractor(codeMetadataProvider),
                    new PluralStringExtractor(codeMetadataProvider)
                });

            foreach (var file in Directory.EnumerateFiles(projectPath, "*.cs", SearchOption.AllDirectories)) {
                if (Path.GetFileName(file).EndsWith(".g.cshtml.cs")) {
                    continue;
                }

                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {
                        var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

                        localizedStringsCollector.Visit(syntaxTree.GetRoot());
                    }
                }
            }

            var razorMetadataProvider = new RazorMetadataProvider(projectBasePath);
            localizedStringsCollector.Extractors = new ILocalizableStringExtractor[] {
                new SingularStringExtractor(razorMetadataProvider),
                new PluralStringExtractor(razorMetadataProvider)
            };

            var compiledViews = ViewCompiler.CompileViews(projectPath);
            var cs = new CSharpCodeProvider();

            foreach (var view in compiledViews) {
                try {
                    var syntaxTree = CSharpSyntaxTree.ParseText(view.GeneratedCode, path: view.FilePath);
                    localizedStringsCollector.Visit(syntaxTree.GetRoot());
                } catch (Exception ex) {
                    Console.WriteLine("Compile fail for: {0}", view.FilePath);
                }
            }

            if (localizedStringsCollector.Strings.Any()) {
                var outputDirectory = Path.GetDirectoryName(outputFilePath);
                Directory.CreateDirectory(outputDirectory);

                using (var potFile = new PotFile(outputFilePath)) {
                    potFile.WriteRecord(localizedStringsCollector.Strings);
                }
            }

            Console.WriteLine($"{Path.GetFileName(projectPath)}: Found {localizedStringsCollector.Strings.Count()} strings.");
        }
    }
}
