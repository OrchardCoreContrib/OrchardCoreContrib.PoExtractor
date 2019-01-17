using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace PoExtractor.Core {
    public class CSharpCodeVisitor : SyntaxWalker {
        public LocalizableStringCollection Strings { get; }
        public IEnumerable<IStringExtractor<SyntaxNode>> Extractors { get; set; }

        public string DefaultContext { get; set; }

        public CSharpCodeVisitor(IEnumerable<IStringExtractor<SyntaxNode>> extractors, LocalizableStringCollection strings) {
            this.Extractors = extractors;
            this.Strings = strings;
        }

        public override void Visit(SyntaxNode node) {
            base.Visit(node);

            foreach (var extractor in Extractors) {
                this.Strings.Add(extractor.TryExtract(node));
            }
        }
    }
}
