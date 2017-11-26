using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PoExtractor.Core.Extractors {
    public class PluralStringExtractor : LocalizableStringExtractor {
        public PluralStringExtractor(ILocalizableMetadataProvider metadataProvider) : base(metadataProvider) {
        }

        public override LocalizableStringOccurence TryExtract(SyntaxNode node) {
            if (node is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax accessor &&
                accessor.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.Text == "T" &&
                accessor.Name.Identifier.Text == "Plural") {

                var arguments = invocation.ArgumentList.Arguments;
                if (arguments.Count >= 2 &&
                    arguments[1].Expression is ArrayCreationExpressionSyntax array) {
                    if (array.Type.ElementType is PredefinedTypeSyntax arrayType &&
                        arrayType.Keyword.Text == "string" &&
                        array.Initializer.Expressions.Count >= 2 &&
                        array.Initializer.Expressions[0] is LiteralExpressionSyntax singularLiteral && singularLiteral.IsKind(SyntaxKind.StringLiteralExpression) &&
                        array.Initializer.Expressions[1] is LiteralExpressionSyntax pluralLiteral && pluralLiteral.IsKind(SyntaxKind.StringLiteralExpression)) {

                        return this.CreateLocalizedString(singularLiteral.Token.ValueText, pluralLiteral.Token.ValueText, node);
                    }
                } else {
                    if (arguments.Count >= 3 &&
                        arguments[1].Expression is LiteralExpressionSyntax singularLiteral && singularLiteral.IsKind(SyntaxKind.StringLiteralExpression) &&
                        arguments[2].Expression is LiteralExpressionSyntax pluralLiteral && pluralLiteral.IsKind(SyntaxKind.StringLiteralExpression)) {

                        return this.CreateLocalizedString(singularLiteral.Token.ValueText, pluralLiteral.Token.ValueText, node);
                    }
                }
            }

            return null;
        }
    }
}
