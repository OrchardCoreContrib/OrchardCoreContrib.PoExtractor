using Fluid.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace PoExtractor {
    class LiquidStatementContext {
        public string FilePath { get; set; }
        public Statement Statement { get; set; }
    }

    class LiquidExpressionContext {
        public string FilePath { get; set; }

        public FilterExpression Expression { get; set; }
    }
}
