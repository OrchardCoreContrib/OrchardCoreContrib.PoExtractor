using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.CS
{
    /// <summary>
    /// Extracts <see cref="LocalizableStringOccurence"/> with the singular text from the C# AST node
    /// </summary>
    /// <remarks>
    /// The localizable string is identified by the name convention - T["TEXT TO TRANSLATE"]
    /// </remarks>
    public class SingularStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="SingularStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
        public SingularStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : base(metadataProvider)
        {
        }

        /// <inheritdoc/>
        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is ElementAccessExpressionSyntax accessor &&
                accessor.Expression is IdentifierNameSyntax identifierName &&
                LocalizerAccessors.LocalizerIdentifiers.Contains(identifierName.Identifier.Text) &&
                accessor.ArgumentList != null)
            {

                var argument = accessor.ArgumentList.Arguments.FirstOrDefault();
                if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    result = new LocalizableStringOccurence()
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
