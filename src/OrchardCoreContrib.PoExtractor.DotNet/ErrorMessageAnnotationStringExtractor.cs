using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Extracts localizable string from data annotations error messages.
/// </summary>
/// <remarks>
/// Creates a new instance of a <see cref="ErrorMessageAnnotationStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public class ErrorMessageAnnotationStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
    : LocalizableStringExtractor<SyntaxNode>(metadataProvider)
{
    private const string ErrorMessageAttributeName = "ErrorMessage";

    /// <inheritdoc/>
    public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        result = null;

        if (node is AttributeSyntax accessor && accessor.ArgumentList != null)
        {
            var argument = accessor.ArgumentList.Arguments
                .Where(a => a.Expression.Parent.ToFullString().StartsWith(ErrorMessageAttributeName))
                .FirstOrDefault();

            if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                result = CreateLocalizedString(literal.Token.ValueText, null, node);
                return true;
            }
        }

        return false;
    }
}
