using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet;

/// <summary>
/// Extracts localizable string from <see cref="DisplayAttribute"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of a <see cref="DisplayAttributeStringExtractor"/>.
/// </remarks>
/// <param name="argumentName">The argument name.</param>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
public abstract class DisplayAttributeStringExtractor(string argumentName, IMetadataProvider<SyntaxNode> metadataProvider)
    : LocalizableStringExtractor<SyntaxNode>(metadataProvider)
{
    private const string DisplayAttributeName = "Display";

    /// <inheritdoc/>
    public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
    {
        result = null;

        if (node is AttributeArgumentSyntax argument
            && argument.Expression.Parent.ToFullString().StartsWith(argumentName)
            && node.Parent?.Parent is AttributeSyntax accessor
            && accessor.Name.ToString() == DisplayAttributeName
            && argument.Expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            result = CreateLocalizedString(literal.Token.ValueText, null, node);
            return true;
        }

        return false;
    }
}
