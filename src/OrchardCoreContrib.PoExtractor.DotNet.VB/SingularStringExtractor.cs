using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB;

/// <summary>
/// Extracts <see cref="LocalizableStringOccurence"/> with the singular text from the C# & VB AST node
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

        if (node is InvocationExpressionSyntax accessor &&
            accessor.Expression is IdentifierNameSyntax identifierName &&
            LocalizerAccessors.LocalizerIdentifiers.Contains(identifierName.Identifier.Text) &&
            accessor.ArgumentList != null)
        {
            var argument = accessor.ArgumentList.Arguments.FirstOrDefault();
            if (argument != null && argument.GetExpression() is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                result = CreateLocalizedString(literal.Token.ValueText, null, node);
                return true;
            }
        }

        return false;
    }
}
