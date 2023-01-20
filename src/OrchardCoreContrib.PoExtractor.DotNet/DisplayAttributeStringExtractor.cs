using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from <see cref="DisplayAttribute"/>.
    /// </summary>
    public abstract class DisplayAttributeStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string DisplayAttributeName = "Display";
        private readonly string _argumentName;

        /// <summary>
        /// Creates a new instance of a <see cref="DisplayAttributeStringExtractor"/>.
        /// </summary>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
        protected DisplayAttributeStringExtractor(string argumentName, IMetadataProvider<SyntaxNode> metadataProvider)
            : base(metadataProvider)
        {
            _argumentName = argumentName;
        }

        /// <inheritdoc/>
        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is AttributeArgumentSyntax argument
                && argument.Expression.Parent.ToFullString().StartsWith(_argumentName)
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
}
