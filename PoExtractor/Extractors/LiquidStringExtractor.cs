using Fluid.Ast;
using PoExtractor.Core;
using PoExtractor.Core.Extractors;
using System;
using System.Collections.Generic;
using System.Text;

namespace PoExtractor.Extractors {
    class LiquidStringExtractor : LocalizableStringExtractor<LiquidExpressionContext> {
        public LiquidStringExtractor(IMetadataProvider<LiquidExpressionContext> metadataProvider) : base(metadataProvider) {
        }

        public override LocalizableStringOccurence TryExtract(LiquidExpressionContext expressionContext) {
            var filter = expressionContext.Expression;

            if (filter.Name == "t") {
                var literal = filter.Input as LiteralExpression;
                if (literal != null) {
                    var text = literal.EvaluateAsync(new Fluid.TemplateContext()).GetAwaiter().GetResult().ToStringValue();
                    return this.CreateLocalizedString(text, null, expressionContext);
                }

            }

            return null;
        }
    }
}
