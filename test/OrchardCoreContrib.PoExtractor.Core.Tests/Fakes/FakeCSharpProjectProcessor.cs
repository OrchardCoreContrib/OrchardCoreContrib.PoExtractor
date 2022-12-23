using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.DotNet.CS;
using OrchardCoreContrib.PoExtractor.DotNet.CS.MetadataProviders;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.Tests.Fakes
{
    public class FakeCSharpProjectProcessor : IProjectProcessor
    {
        private static readonly string _defaultPath = "ProjectFiles";

        public void Process(string path, string basePath, LocalizableStringCollection strings)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = _defaultPath;
            }

            if (string.IsNullOrEmpty(basePath))
            {
                basePath = _defaultPath;
            }

            var codeMetadataProvider = new CSharpMetadataProvider(basePath);
            var csharpWalker = new ExtractingCodeWalker(
                new IStringExtractor<SyntaxNode>[] {
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
