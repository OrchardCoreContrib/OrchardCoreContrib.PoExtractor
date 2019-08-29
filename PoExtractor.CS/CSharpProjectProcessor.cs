using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.CS.MetadataProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PoExtractor.CS {
    /// <summary>
    /// Extracts localizable strings from all *.cs files in the project path and *.cshtml files in the folde Views under the project path
    /// </summary>
    public class CSharpProjectProcessor : IProjectProcessor {
        public void Process(string path, string basePath, LocalizableStringCollection strings) {
            /* C# */
            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var csharpWalker = new ExtractingCodeWalker(
                new IStringExtractor<SyntaxNode>[] {
                        new SingularStringExtractor(codeMetadataProvider),
                        new PluralStringExtractor(codeMetadataProvider)
                }, strings);

            foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories)) {
                if (Path.GetFileName(file).EndsWith(".cshtml.g.cs")) {
                    continue;
                }

                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {
                        var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

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

            foreach (var view in compiledViews) {
                try {
                    var syntaxTree = CSharpSyntaxTree.ParseText(view.GeneratedCode, path: view.FilePath);
                    razorWalker.Visit(syntaxTree.GetRoot());
                } catch (Exception) {
                    Console.WriteLine("Process failed for: {0}", view.FilePath);
                }
            }
        }
    }
}
