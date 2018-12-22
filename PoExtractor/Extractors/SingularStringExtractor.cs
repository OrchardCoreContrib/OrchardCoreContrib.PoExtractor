using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PoExtractor.Core.Extractors {
    public class SingularStringExtractor : LocalizableStringExtractor<SyntaxNode> {
        public SingularStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : base(metadataProvider) {
        }

        public override LocalizableStringOccurence TryExtract(SyntaxNode node) {
            if (node is ElementAccessExpressionSyntax accessor &&
                accessor.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.Text == "T" &&
                accessor.ArgumentList != null) {

                var argument = accessor.ArgumentList.Arguments.FirstOrDefault();
                if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression)) {

                    return this.CreateLocalizedString(literal.Token.ValueText, null, node);
                }
            }

            return null;
        }
    }
}
