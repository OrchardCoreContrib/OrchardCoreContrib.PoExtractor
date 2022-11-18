using Fluid.Ast;

namespace OrchardCoreContrib.PoExtractor.Liquid
{
    class LiquidStatementContext
    {
        public string FilePath { get; set; }
        public Statement Statement { get; set; }
    }

    class LiquidExpressionContext
    {
        public string FilePath { get; set; }

        public FilterExpression Expression { get; set; }
    }
}
