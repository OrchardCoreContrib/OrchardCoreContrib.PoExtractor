using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet.VB
{
    /// <summary>
    /// Extracts <see cref="LocalizableStringOccurence"/> with the singular text from the C# & VB AST node
    /// </summary>
    /// <remarks>
    /// The localizable string is identified by the name convention - T["TEXT TO TRANSLATE"]
    /// </remarks>
    public class SingularStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        public SingularStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : base(metadataProvider)
        {

        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is InvocationExpressionSyntax accessor &&
                accessor.Expression is IdentifierNameSyntax identifierName &&
                LocalizerAccessors.LocalizerIdentifiers.Contains(identifierName.Identifier.Text) &&
                accessor.ArgumentList != null)
            {
                var argument = accessor.ArgumentList.Arguments.FirstOrDefault();
                if (argument != null && argument.GetExpression() is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    result = new LocalizableStringOccurence()
                    {
                        Text = literal.Token.ValueText,
                        Context = this.MetadataProvider.GetContext(node),
                        Location = this.MetadataProvider.GetLocation(node)
                    };

                    return true;
                }
            }

            return false;
        }
    }
}
