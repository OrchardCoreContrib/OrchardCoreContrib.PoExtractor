using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS
{
    /// <summary>
    /// Extracts localizable strings from all *.cs files in the project path.
    /// </summary>
    public class CSharpProjectProcessor : IProjectProcessor
    {
        /// <inheritdoc/>
        public virtual void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var csharpWalker = new ExtractingCodeWalker(new IStringExtractor<SyntaxNode>[]
            {
                new SingularStringExtractor(codeMetadataProvider),
                new PluralStringExtractor(codeMetadataProvider),
                new ErrorMessageAnnotationStringExtractor(codeMetadataProvider),
                new DisplayAttributeDescriptionStringExtractor(codeMetadataProvider),
                new DisplayAttributeNameStringExtractor(codeMetadataProvider),
                new DisplayAttributeGroupNameStringExtractor(codeMetadataProvider),
                new DisplayAttributeShortNameStringExtractor(codeMetadataProvider)
            }, strings);

            foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).OrderBy(file => file))
            {
                if (file.StartsWith(Path.Combine(path, "obj")))
                {
                    continue;
                }

                using (var stream = File.OpenRead(file))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

                        csharpWalker.Visit(syntaxTree.GetRoot());
                    }
                }
            }
        }
    }
}
