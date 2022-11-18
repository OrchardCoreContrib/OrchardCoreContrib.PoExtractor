using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.Core;
using OrchardCoreContrib.PoExtractor.Core.Contracts;
using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.Razor.MetadataProviders;

namespace OrchardCoreContrib.PoExtractor.Razor
{
    /// <summary>
    /// Extracts localizable strings from all *.cshtml files in the folder Views under the project path
    /// </summary>
    public abstract class RazorViewsProcessor : IProjectProcessor
    {
        public virtual void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            var razorMetadataProvider = new RazorMetadataProvider(basePath);
            var razorWalker = new ExtractingCodeWalker(GetStringExtractors(razorMetadataProvider), strings);
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

        protected abstract IStringExtractor<SyntaxNode>[] GetStringExtractors(RazorMetadataProvider razorMetadataProvider);
    }
}
