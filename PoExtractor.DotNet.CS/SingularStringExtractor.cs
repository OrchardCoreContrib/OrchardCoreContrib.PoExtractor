using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet.CS
{
    /// <summary>
    /// Extracts <see cref="LocalizableStringOccurence"/> with the singular text from the C# AST node
    /// </summary>
    /// <remarks>
    /// The localizable string is identified by the name convention - T["TEXT TO TRANSLATE"]
    /// </remarks>
    public class SingularStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private readonly string[] localizerIdentifiers;

        public SingularStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider, string identifier) : base(metadataProvider)
        {
            localizerIdentifiers = LocalizerAccessors.LocalizerIdentifiers.Append(identifier).ToArray();
        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is ElementAccessExpressionSyntax accessor)

                if (accessor.Expression is IdentifierNameSyntax identifierName)
                    if (localizerIdentifiers.Contains(identifierName.Identifier.Text))
                        if (accessor.ArgumentList != null)
                        {

                            var argument = accessor.ArgumentList.Arguments.FirstOrDefault();
                            if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
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
