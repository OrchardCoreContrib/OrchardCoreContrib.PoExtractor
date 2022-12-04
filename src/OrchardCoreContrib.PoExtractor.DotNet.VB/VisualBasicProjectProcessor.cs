using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB
{
    /// <summary>
    /// Extracts localizable strings from all *.vb files in the project path.
    /// </summary>
    public class VisualBasicProjectProcessor : IProjectProcessor
    {
        /// <inheritdoc/>
        public void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var visualBasicWalker = new ExtractingCodeWalker(new IStringExtractor<SyntaxNode>[]
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
                using (var stream = File.OpenRead(file))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var syntaxTree = VisualBasicSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

                        visualBasicWalker.Visit(syntaxTree.GetRoot());
                    }
                }
            }
        }
    }
}
