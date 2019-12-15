using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.VB.MetadataProviders;

namespace PoExtractor.VB
{
    /// <summary>
    /// Extracts localizable strings from all *.vb files in the project path and *.cshtml files in the folde Views under the project path
    /// </summary>
    public class VisualBasicProjectProcessor : IProjectProcessor {
        public void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            /* VB */
            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var csharpWalker = new ExtractingCodeWalker(
                new IStringExtractor<SyntaxNode>[]
                {
                        new SingularStringExtractor(codeMetadataProvider),
                        new PluralStringExtractor(codeMetadataProvider)
                }, strings);

            foreach (var file in Directory.EnumerateFiles(path, "*.vb", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file).EndsWith(".cshtml.g.cs"))
                {
                    continue;
                }

                using (var stream = File.OpenRead(file))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var syntaxTree = VisualBasicSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

                        csharpWalker.Visit(syntaxTree.GetRoot());
                    }
                }
            }

            /* CSHTML */
            var razorMetadataProvider = new RazorMetadataProvider(basePath);
            var razorWalker = new ExtractingCodeWalker(
                new IStringExtractor<SyntaxNode>[] {
                        new SingularStringExtractor(razorMetadataProvider),
                        new PluralStringExtractor(razorMetadataProvider)
                }, strings);

            var compiledViews = ViewCompiler.CompileViews(path);

            foreach (var view in compiledViews)
            {
                try
                {
                    var syntaxTree = CSharpSyntaxTree.ParseText(view.GeneratedCode, path: view.FilePath);
                    razorWalker.Visit(syntaxTree.GetRoot());
                }
                catch
                {
                    Console.WriteLine("Process failed for: {0}", view.FilePath);
                }
            }
        }
    }
}
