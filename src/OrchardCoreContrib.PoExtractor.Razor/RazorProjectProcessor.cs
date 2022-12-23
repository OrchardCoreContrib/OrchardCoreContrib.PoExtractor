using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.DotNet.CS;
using OrchardCoreContrib.PoExtractor.Razor.MetadataProviders;
using System;

namespace OrchardCoreContrib.PoExtractor.Razor
{
    /// <summary>
    /// Extracts localizable strings from all *.cshtml files in the folder Views under the project path
    /// </summary>
    public class RazorProjectProcessor : CSharpProjectProcessor
    {
        /// <inheritdoc/>
        public override void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            var razorMetadataProvider = new RazorMetadataProvider(basePath);
            var razorWalker = new ExtractingCodeWalker(new IStringExtractor<SyntaxNode>[]
            {
                new SingularStringExtractor(razorMetadataProvider),
                new PluralStringExtractor(razorMetadataProvider),
                new ErrorMessageAnnotationStringExtractor(razorMetadataProvider),
                new DisplayAttributeDescriptionStringExtractor(razorMetadataProvider),
                new DisplayAttributeNameStringExtractor(razorMetadataProvider),
                new DisplayAttributeGroupNameStringExtractor(razorMetadataProvider),
                new DisplayAttributeShortNameStringExtractor(razorMetadataProvider)
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
