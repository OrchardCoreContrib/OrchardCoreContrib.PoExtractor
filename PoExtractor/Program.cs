using System;
using System.IO;
using System.Linq;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.DotNet;
using PoExtractor.DotNet.CS;
using PoExtractor.DotNet.VB;

namespace PoExtractor {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 2) {
                WriteHelp();
                return;
            }

            var basePath = args[0];
            var outputBasePath = args[1];

            string[] projectFiles;
            if (Directory.Exists(basePath)) {
                projectFiles = Directory.EnumerateFiles(basePath, $"*{ProjectExtension.CS}", SearchOption.AllDirectories).OrderBy(file => file)
                    .Union(Directory.EnumerateFiles(basePath, $"*{ProjectExtension.VB}", SearchOption.AllDirectories).OrderBy(file => file)).ToArray();
            } else {
                WriteHelp();
                return;
            }

            var processors = new IProjectProcessor[] {
                new CSharpProjectProcessor(),
                new VisualBasicProjectProcessor()
            };

            foreach (var projectFilePath in projectFiles) {
                var projectPath = Path.GetDirectoryName(projectFilePath);
                var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var projectRelativePath = projectPath.TrimStart(basePath + Path.DirectorySeparatorChar);
                var outputPath = Path.Combine(outputBasePath, Path.GetFileNameWithoutExtension(projectFilePath) + PoWriter.PortaleObjectTemplateExtension);

                var strings = new LocalizableStringCollection();
                foreach (var processor in processors) {
                    processor.Process(projectPath, projectBasePath, strings);
                }

                if (strings.Values.Any()) {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                    using (var potFile = new PoWriter(outputPath)) {
                        potFile.WriteRecord(strings.Values);
                    }
                }

                Console.WriteLine($"{Path.GetFileName(projectPath)}: Found {strings.Values.Count()} strings.");
            }
        }

        private static void WriteHelp() {
            Console.WriteLine("Usage: extractpo input output");
            Console.WriteLine("    input: path to the input directory, all projects at the the path will be processed");
            Console.WriteLine("    output: path to a directory where POT files will be generated");
        }
    }
}
