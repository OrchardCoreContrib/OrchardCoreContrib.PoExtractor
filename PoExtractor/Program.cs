using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Razor;
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
            if (args[0].EndsWith(".csproj")) {
                projectFiles = new string[] { args[0] };
            } else if (Directory.Exists(args[0])) {
                projectFiles = Directory.EnumerateFiles(args[0], "*.csproj", SearchOption.AllDirectories).ToArray();
            } else {
                WriteHelp();
                return;
            }

            var outputBasePath = args[1];

            foreach (var projectFilePath in projectFiles) {
                var projectPath = Path.GetDirectoryName(projectFilePath);
                var basePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var outputPath = Path.Combine(outputBasePath, projectPath.TrimStart(basePath), "OrchardCore.pot");

                ProcessProject(projectPath, basePath, outputPath);
            }
        }

        private static void WriteHelp() {
            Console.WriteLine("Usage: PoExtractor.exe input output");
            Console.WriteLine("    input: Path to a .cproj file to proccess single project or a path to prcess all project in the specified path");
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

            var razorHost = new RazorEngineHost(RazorCodeLanguage.Languages["cshtml"]);
            var razor = new RazorTemplateEngine(razorHost);
            var cs = new CSharpCodeProvider();

            foreach (var file in Directory.EnumerateFiles(projectPath, "*.cshtml", SearchOption.AllDirectories)) {
                using (var reader = new StreamReader(file)) {
                    try {
                        var compileUnit = razor.GenerateCode(reader, "View", "PoExtraxtor.GeneratedCode", file, null).GeneratedCode;

                        var sb = new StringBuilder();
                        using (var writer = new StringWriter(sb)) {
                            cs.GenerateCodeFromCompileUnit(compileUnit, writer, new System.CodeDom.Compiler.CodeGeneratorOptions());
                        }

                        var syntaxTree = CSharpSyntaxTree.ParseText(sb.ToString(), path: file);

                        localizedStringsCollector.Visit(syntaxTree.GetRoot());
                    } catch (Exception ex) {
                        Console.WriteLine("Compile fail for: {0}", file);
                    }
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
