using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.Razor;
using PoExtractor.Razor.MetadataProviders;
using PoExtractor.DotNet.VB.MetadataProviders;

namespace PoExtractor.DotNet.VB
{
    /// <summary>
    /// Extracts localizable strings from all *.vb files in the project path and *.cshtml files in the folder Views under the project path
    /// </summary>
    public class VisualBasicProjectProcessor : RazorViewsProcessor
    {
        public override void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            /* VB */
            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var csharpWalker = new ExtractingCodeWalker(
                new IStringExtractor<SyntaxNode>[]
                {
                    new SingularStringExtractor(codeMetadataProvider),
                    new PluralStringExtractor(codeMetadataProvider),
                    new ErrorMessageAnnotationStringExtractor(codeMetadataProvider),
                    new DisplayAttributeDescriptionStringExtractor(codeMetadataProvider),
                    new DisplayAttributeNameStringExtractor(codeMetadataProvider),
                    new DisplayAttributeGroupNameStringExtractor(codeMetadataProvider),
                    new DisplayAttributeShortNameStringExtractor(codeMetadataProvider)
                }, strings);

            foreach (var file in Directory.EnumerateFiles(path, "*.vb", SearchOption.AllDirectories).OrderBy(file => file))
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

            base.Process(path, basePath, strings);
        }

        protected override IStringExtractor<SyntaxNode>[] GetStringExtractors(RazorMetadataProvider razorMetadataProvider)
            => new IStringExtractor<SyntaxNode>[]
            {
                new SingularStringExtractor(razorMetadataProvider),
                new PluralStringExtractor(razorMetadataProvider),
                new ErrorMessageAnnotationStringExtractor(razorMetadataProvider),
                new DisplayAttributeDescriptionStringExtractor(razorMetadataProvider),
                new DisplayAttributeNameStringExtractor(razorMetadataProvider),
                new DisplayAttributeGroupNameStringExtractor(razorMetadataProvider),
                new DisplayAttributeShortNameStringExtractor(razorMetadataProvider)
            };
    }
}
