using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS;

/// <summary>
/// Extracts <see cref="LocalizableStringOccurence"/> with the singular text from the C# AST node
/// </summary>
/// <remarks>
/// The localizable string is identified by the name convention - T.Plural(count, "1 book", "{0} books")
/// </remarks>
/// <remarks>
/// Creates a new instance of a <see cref="PluralStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class PluralStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : LocalizableStringExtractor<SyntaxNode>(metadataProvider)
{

    /// <inheritdoc/>
    public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
    {
        ArgumentNullException.ThrowIfNull(nameof(node));

        result = null;

        if (node is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax accessor &&
            accessor.Expression is IdentifierNameSyntax identifierName &&
            LocalizerAccessors.LocalizerIdentifiers.Contains(identifierName.Identifier.Text) &&
            accessor.Name.Identifier.Text == "Plural")
        {

            var arguments = invocation.ArgumentList.Arguments;
            if (arguments.Count >= 2 &&
                arguments[1].Expression is ArrayCreationExpressionSyntax array)
            {
                if (array.Type.ElementType is PredefinedTypeSyntax arrayType &&
                    arrayType.Keyword.Text == "string" &&
                    array.Initializer.Expressions.Count >= 2 &&
                    array.Initializer.Expressions[0] is LiteralExpressionSyntax singularLiteral && singularLiteral.IsKind(SyntaxKind.StringLiteralExpression) &&
                    array.Initializer.Expressions[1] is LiteralExpressionSyntax pluralLiteral && pluralLiteral.IsKind(SyntaxKind.StringLiteralExpression))
                {

                    result = CreateLocalizedString(singularLiteral.Token.ValueText, pluralLiteral.Token.ValueText, node);

                    return true;
                }
            }
            else
            {
                if (arguments.Count >= 3 &&
                    arguments[1].Expression is LiteralExpressionSyntax singularLiteral && singularLiteral.IsKind(SyntaxKind.StringLiteralExpression) &&
                    arguments[2].Expression is LiteralExpressionSyntax pluralLiteral && pluralLiteral.IsKind(SyntaxKind.StringLiteralExpression))
                {

                    result = CreateLocalizedString(singularLiteral.Token.ValueText, pluralLiteral.Token.ValueText, node);

                    return true;
                }
            }
        }

        return false;
    }
}
