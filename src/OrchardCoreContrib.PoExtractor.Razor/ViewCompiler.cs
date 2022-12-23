using Microsoft.AspNetCore.Razor.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.Razor
{
    /// <summary>
    /// Represents a utility class to compile razor views.
    /// </summary>
    public static class ViewCompiler
    {
        /// <summary>
        /// Complies the views on a given project.
        /// </summary>
        /// <param name="projectDirectory">The project directory.</param>
        public static IEnumerable<RazorPageGeneratorResult> CompileViews(string projectDirectory)
        {
            if (string.IsNullOrEmpty(projectDirectory))
            {
                throw new ArgumentException($"'{nameof(projectDirectory)}' cannot be null or empty.", nameof(projectDirectory));
            }

            var projectEngine = CreateProjectEngine("OrchardCoreContrib.PoExtractor.GeneratedCode", projectDirectory);

            foreach (var item in projectEngine.FileSystem.EnumerateItems(projectDirectory).OrderBy(rzrProjItem => rzrProjItem.FileName))
            {
                yield return GenerateCodeFile(projectEngine, item);
            }
        }

        private static RazorProjectEngine CreateProjectEngine(string rootNamespace, string projectDirectory)
        {
            if (string.IsNullOrEmpty(rootNamespace))
            {
                throw new ArgumentException($"'{nameof(rootNamespace)}' cannot be null or empty.", nameof(rootNamespace));
            }

            if (string.IsNullOrEmpty(projectDirectory))
            {
                throw new ArgumentException($"'{nameof(projectDirectory)}' cannot be null or empty.", nameof(projectDirectory));
            }

            var fileSystem = RazorProjectFileSystem.Create(projectDirectory);
            var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, builder =>
            {

                builder
                    .SetNamespace(rootNamespace)
                    .ConfigureClass((document, @class) =>
                    {
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
