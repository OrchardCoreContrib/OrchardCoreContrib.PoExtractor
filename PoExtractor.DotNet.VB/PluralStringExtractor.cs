using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet.VB
{
    /// <summary>
    /// Extracts <see cref="LocalizableStringOccurence"/> with the singular text from the VB AST node
    /// </summary>
    /// <remarks>
    /// The localizable string is identified by the name convention - T.Plural(count, "1 book", "{0} books")
    /// </remarks>
    public class PluralStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        public PluralStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : base(metadataProvider)
        {

        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax accessor &&
                accessor.Expression is IdentifierNameSyntax identifierName &&
                LocalizerAccessors.LocalizerIdentifiers.Contains(identifierName.Identifier.Text) &&
                accessor.Name.Identifier.Text == "Plural")
            {
                var arguments = invocation.ArgumentList.Arguments;
                if (arguments.Count >= 2 &&
                    arguments[1].GetExpression() is ArrayCreationExpressionSyntax array) {
                    if (array.Type is PredefinedTypeSyntax arrayType &&
                        arrayType.Keyword.Text == "String" &&
                        array.Initializer.Initializers.Count >= 2 &&
                        array.Initializer.Initializers.ElementAt(0) is LiteralExpressionSyntax singularLiteral && singularLiteral.IsKind(SyntaxKind.StringLiteralExpression) &&
                        array.Initializer.Initializers.ElementAt(1) is LiteralExpressionSyntax pluralLiteral && pluralLiteral.IsKind(SyntaxKind.StringLiteralExpression)) {

                        result = this.CreateLocalizedString(singularLiteral.Token.ValueText, pluralLiteral.Token.ValueText, node);
                        return true;
                    }
                } else
                {
                    if (arguments.Count >= 3 &&
                        arguments[1].GetExpression() is LiteralExpressionSyntax singularLiteral && singularLiteral.IsKind(SyntaxKind.StringLiteralExpression) &&
                        arguments[2].GetExpression() is LiteralExpressionSyntax pluralLiteral && pluralLiteral.IsKind(SyntaxKind.StringLiteralExpression)) {

                        result = this.CreateLocalizedString(singularLiteral.Token.ValueText, pluralLiteral.Token.ValueText, node);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
