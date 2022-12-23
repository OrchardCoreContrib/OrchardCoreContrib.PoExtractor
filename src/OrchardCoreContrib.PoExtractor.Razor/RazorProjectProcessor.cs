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
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException($"'{nameof(basePath)}' cannot be null or empty.", nameof(basePath));
            }

            if (strings is null)
            {
                throw new ArgumentNullException(nameof(strings));
            }

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
