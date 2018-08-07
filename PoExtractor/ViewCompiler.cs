using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace PoExtractor {
    static class ViewCompiler {
        public static IList<RazorPageGeneratorResult> CompileViews(string projectDirectory) {
            var projectEngine = CreateProjectEngine("PoExtraxtor.GeneratedCode", projectDirectory);

            var results = new List<RazorPageGeneratorResult>();

            var viewDirectories = Directory.EnumerateDirectories(projectDirectory, "Views", SearchOption.AllDirectories);
            foreach (var viewDir in viewDirectories) {
                var viewDirPath = viewDir.Substring(projectDirectory.Length).Replace('\\', '/');
                var viewFiles = projectEngine.FileSystem.EnumerateItems(viewDirPath);

                foreach (var item in viewFiles.Where(o => o.Extension == ".cshtml")) {
                    results.Add(GenerateCodeFile(projectEngine, item));
                }
            }

            return results;
        }

        static RazorProjectEngine CreateProjectEngine(string rootNamespace, string projectDirectory) {
            var fileSystem = RazorProjectFileSystem.Create(projectDirectory);
            var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, builder => {

                builder
                    .SetNamespace(rootNamespace)
                    .ConfigureClass((document, @class) => {
                        @class.ClassName = Path.GetFileNameWithoutExtension(document.Source.FilePath);
                    });

                FunctionsDirective.Register(builder);
                InheritsDirective.Register(builder);
                SectionDirective.Register(builder);
            });

            return projectEngine;
        }

        static RazorPageGeneratorResult GenerateCodeFile(RazorProjectEngine projectEngine, RazorProjectItem projectItem) {
            var codeDocument = projectEngine.Process(projectItem);
            var cSharpDocument = codeDocument.GetCSharpDocument();

            return new RazorPageGeneratorResult {
                FilePath = projectItem.PhysicalPath,
                GeneratedCode = cSharpDocument.GeneratedCode,
            };
        }
    }

    class RazorPageGeneratorResult {
        public string FilePath { get; set; }
        public string GeneratedCode { get; set; }
    }
}
