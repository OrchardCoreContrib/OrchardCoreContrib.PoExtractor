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
/// The localizable string is identified by the name convention - T["TEXT TO TRANSLATE"]
/// </remarks>
/// <remarks>
/// Creates a new instance of a <see cref="SingularStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class SingularStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : LocalizableStringExtractor<SyntaxNode>(metadataProvider)
{

    /// <inheritdoc/>
    public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
    {
        ArgumentNullException.ThrowIfNull(node);

        result = null;

        if (node is ElementAccessExpressionSyntax accessor &&
            accessor.Expression is IdentifierNameSyntax identifierName &&
            LocalizerAccessors.LocalizerIdentifiers.Contains(identifierName.Identifier.Text) &&
            accessor.ArgumentList != null)
        {

            var argument = accessor.ArgumentList.Arguments.FirstOrDefault();
            if (argument != null && TryGetString(argument.Expression, out var value))
            {
                result = CreateLocalizedString(value, null, node);
                return true;
            }
        }

        return false;
    }

    private static bool TryGetString(ExpressionSyntax expression, out string value)
    {
        if (expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            value = literal.Token.ValueText;
            return true;
        }

        if (expression is BinaryExpressionSyntax binary &&
            binary.IsKind(SyntaxKind.AddExpression) &&
            TryGetString(binary.Left, out var left) &&
            TryGetString(binary.Right, out var right))
        {
            value = left + right;
            return true;
        }

        value = null;
        return false;
    }
}
