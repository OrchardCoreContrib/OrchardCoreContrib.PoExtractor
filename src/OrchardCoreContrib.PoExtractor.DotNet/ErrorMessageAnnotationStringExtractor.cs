using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from data annotations error messages.
    /// </summary>
    public class ErrorMessageAnnotationStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string ErrorMessageAttributeName = "ErrorMessage";

        /// <summary>
        /// Creates a new instance of a <see cref="ErrorMessageAnnotationStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
        public ErrorMessageAnnotationStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base(metadataProvider)
        {

        }

        /// <inheritdoc/>
        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            result = null;

            if (node is AttributeSyntax accessor && accessor.ArgumentList != null)
            {
                var argument = accessor.ArgumentList.Arguments
                    .Where(a => a.Expression.Parent.ToFullString().StartsWith(ErrorMessageAttributeName))
                    .FirstOrDefault();
                
                if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    result = new LocalizableStringOccurence
                    {
                        Text = literal.Token.ValueText,
                        Context = MetadataProvider.GetContext(node),
                        Location = MetadataProvider.GetLocation(node)
                    };

                    return true;
                }
            }

            return false;
        }
    }
}
