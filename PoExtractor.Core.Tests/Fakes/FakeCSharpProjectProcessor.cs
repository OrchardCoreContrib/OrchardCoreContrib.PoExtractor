using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PoExtractor.Core.Contracts;
using PoExtractor.DotNet;
using PoExtractor.DotNet.CS;
using PoExtractor.DotNet.CS.MetadataProviders;

namespace PoExtractor.Core.Tests.Fakes
{
    public class FakeCSharpProjectProcessor : IProjectProcessor
    {
        private static readonly string _defaultPath = "ProjectFiles";

        private readonly string identifier;

        public FakeCSharpProjectProcessor(string identifier)
        {
            this.identifier = identifier;
        }

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

            var codeMetadataProvider = new CodeMetadataProvider(basePath);
            var csharpWalker = new ExtractingCodeWalker(
                new IStringExtractor<SyntaxNode>[] {
                        new SingularStringExtractor(codeMetadataProvider, identifier),
                        new PluralStringExtractor(codeMetadataProvider, identifier),
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
