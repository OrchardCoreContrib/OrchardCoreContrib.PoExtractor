using Fluid.Ast;

namespace OrchardCoreContrib.PoExtractor.Liquid;

/// <summary>
/// Represents a liquid statement context.
/// </summary>
public class LiquidStatementContext
{
    /// <summary>
    /// Gets or sets liquid file path.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the liquid statement.
    /// </summary>
    public Statement Statement { get; set; }
}
