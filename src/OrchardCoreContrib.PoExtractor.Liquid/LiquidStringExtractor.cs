using Fluid.Ast;

namespace OrchardCoreContrib.PoExtractor.Liquid
{
    /// <summary>
    /// Extracts localizable strings the Fluid AST node
    /// </summary>
    /// <remarks>
    /// The localizable string is identified by the name convention of the filter - "TEXT TO TRANSLATE" | t 
    /// </remarks>
    class LiquidStringExtractor : LocalizableStringExtractor<LiquidExpressionContext>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="LiquidStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
        public LiquidStringExtractor(IMetadataProvider<LiquidExpressionContext> metadataProvider) : base(metadataProvider)
        {
        }

        /// <inheritdoc/>
        public override bool TryExtract(LiquidExpressionContext expressionContext, out LocalizableStringOccurence result)
        {
            result = null;
            var filter = expressionContext.Expression;

            if (filter.Name == "t")
            {
                if (filter.Input is LiteralExpression literal)
                {
                    var text = literal.EvaluateAsync(new Fluid.TemplateContext()).GetAwaiter().GetResult().ToStringValue();
                    result = this.CreateLocalizedString(text, null, expressionContext);
                    return true;
                }
            }

            return false;
        }
    }
}
