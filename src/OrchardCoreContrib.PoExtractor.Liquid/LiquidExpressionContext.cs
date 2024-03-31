using Fluid.Ast;

namespace OrchardCoreContrib.PoExtractor.Liquid;

/// <summary>
/// Represents a liquid expression context.
/// </summary>
public class LiquidExpressionContext
{
    /// <summary>
    /// Gets or sets the liquid file path.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the expression.
    /// </summary>
    public FilterExpression Expression { get; set; }
}
