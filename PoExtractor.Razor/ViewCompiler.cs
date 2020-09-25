using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace PoExtractor.Razor
{
    public static class ViewCompiler
    {
        public static IList<RazorPageGeneratorResult> CompileViews(string projectDirectory)
        {
            var projectEngine = CreateProjectEngine("PoExtractor.GeneratedCode", projectDirectory);

            var results = new List<RazorPageGeneratorResult>();

            // MVC uses the Views folder as convention.
            // Razor Pages uses the Pages folder as convention. 
            // Both use Areas as convention and either have Pages or Views as subfolders.
            var viewDirectories = Directory.EnumerateDirectories(projectDirectory, "Views", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateDirectories(projectDirectory, "Pages", SearchOption.AllDirectories))
                .Distinct()
                .OrderBy(dirName => dirName);
            foreach (var viewDir in viewDirectories)
            {
                var viewDirPath = viewDir.Substring(projectDirectory.Length).Replace('\\', '/');
                var viewFiles = projectEngine.FileSystem.EnumerateItems(viewDirPath).OrderBy(rzrProjItem => rzrProjItem.FileName);

                foreach (var item in viewFiles.Where(o => o.Extension == ".cshtml"))
                {
                    results.Add(GenerateCodeFile(projectEngine, item));
                }
            }

            return results;
        }

        public static RazorProjectEngine CreateProjectEngine(string rootNamespace, string projectDirectory)
        {
            var fileSystem = RazorProjectFileSystem.Create(projectDirectory);
            var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, builder =>
            {

                builder
                    .SetNamespace(rootNamespace)
                    .ConfigureClass((document, @class) => {
                        @class.ClassName = Path.GetFileNameWithoutExtension(document.Source.FilePath);
                    });
#if NETSTANDARD2_0
                FunctionsDirective.Register(builder);
                InheritsDirective.Register(builder);
                SectionDirective.Register(builder);
#endif
            });

            return projectEngine;
        }

        public static RazorPageGeneratorResult GenerateCodeFile(RazorProjectEngine projectEngine, RazorProjectItem projectItem)
        {
            var codeDocument = projectEngine.Process(projectItem);
            var cSharpDocument = codeDocument.GetCSharpDocument();

            return new RazorPageGeneratorResult
            {
                FilePath = projectItem.PhysicalPath,
                GeneratedCode = cSharpDocument.GeneratedCode,
            };
        }
    }
}
