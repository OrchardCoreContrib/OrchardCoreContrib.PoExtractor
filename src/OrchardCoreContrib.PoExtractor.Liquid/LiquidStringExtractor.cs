using Fluid.Ast;
using System;

namespace OrchardCoreContrib.PoExtractor.Liquid;

/// <summary>
/// Extracts localizable strings the Fluid AST node
/// </summary>
/// <remarks>
/// The localizable string is identified by the name convention of the filter - "TEXT TO TRANSLATE" | t 
/// </remarks>
/// <remarks>
/// Creates a new instance of a <see cref="LiquidStringExtractor"/>.
/// </remarks>
/// <param name="metadataProvider">The <see cref="IMetadataProvider{T}"/>.</param>
public class LiquidStringExtractor(IMetadataProvider<LiquidExpressionContext> metadataProvider)
    : LocalizableStringExtractor<LiquidExpressionContext>(metadataProvider)
{
    private static readonly string _localizationFilterName = "t";

    /// <inheritdoc/>
    public override bool TryExtract(LiquidExpressionContext expressionContext, out LocalizableStringOccurence result)
    {
        ArgumentNullException.ThrowIfNull(expressionContext);

        result = null;
        var filter = expressionContext.Expression;

        if (filter.Name == _localizationFilterName)
        {
            if (filter.Input is LiteralExpression literal)
            {
                var text = literal
                    .EvaluateAsync(new Fluid.TemplateContext())
                    .GetAwaiter()
                    .GetResult()
                    .ToStringValue();

                result = CreateLocalizedString(text, null, expressionContext);

                return true;
            }
        }

        return false;
    }
}
