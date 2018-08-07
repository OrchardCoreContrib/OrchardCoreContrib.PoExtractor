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
        static void Main(string[] args) {
            if (args.Length != 2) {
                WriteHelp();
                return;
            }

            string[] projectFiles;
            if (Directory.Exists(args[0])) {
                projectFiles = Directory.EnumerateFiles(args[0], "*.csproj", SearchOption.AllDirectories).ToArray();
            } else {
                WriteHelp();
                return;
            }

            var outputBasePath = args[1];

            foreach (var projectFilePath in projectFiles) {
                var projectPath = Path.GetDirectoryName(projectFilePath);
                var basePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var outputPath = Path.Combine(outputBasePath, projectPath.TrimStart(basePath) + ".pot");

                ProcessProject(projectPath, basePath, outputPath);
            }
        }

        private static void WriteHelp() {
            Console.WriteLine("Usage: PoExtractor.exe input output");
            Console.WriteLine("    input: Path to input directory, all projects at the the path will be processed");
            Console.WriteLine("    output: Path to a directory where POT files will be generated");
        }

        private static void ProcessProject(string projectPath, string basePath, string outputFilePath) {
            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var localizedStringsCollector = new LocalizableStringCollector(
                new ILocalizableStringExtractor[] {
                    new SingularStringExtractor(codeMetadataProvider),
                    new PluralStringExtractor(codeMetadataProvider)
                });

            foreach (var file in Directory.EnumerateFiles(projectPath, "*.cs", SearchOption.AllDirectories)) {
                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {
                        var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

                        localizedStringsCollector.Visit(syntaxTree.GetRoot());
                    }
                }
            }

            var razorMetadataProvider = new RazorMetadataProvider(basePath);
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

            Console.WriteLine($"{projectPath.TrimStart(basePath)}: Found {localizedStringsCollector.Strings.Count()} strings.");
        }
    }
}
