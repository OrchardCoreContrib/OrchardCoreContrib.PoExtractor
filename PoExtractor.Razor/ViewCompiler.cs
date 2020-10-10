using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace PoExtractor.Razor
{
    public static class ViewCompiler
    {
        public static IEnumerable<RazorPageGeneratorResult> CompileViews(string projectDirectory)
        {
            var projectEngine = CreateProjectEngine("PoExtractor.GeneratedCode", projectDirectory);

            foreach (var item in projectEngine.FileSystem.EnumerateItems(projectDirectory).OrderBy(rzrProjItem => rzrProjItem.FileName))
            {
                yield return GenerateCodeFile(projectEngine, item);
            }
        }

        private static RazorProjectEngine CreateProjectEngine(string rootNamespace, string projectDirectory)
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

        private static RazorPageGeneratorResult GenerateCodeFile(RazorProjectEngine projectEngine, RazorProjectItem projectItem)
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
