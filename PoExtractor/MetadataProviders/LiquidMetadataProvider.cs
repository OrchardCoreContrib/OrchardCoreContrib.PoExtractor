using PoExtractor.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PoExtractor.MetadataProviders {
    class LiquidMetadataProvider : IMetadataProvider<LiquidExpressionContext> {
        public string BasePath { get; set; }

        public LiquidMetadataProvider(string basePath) {
            this.BasePath = basePath;
        }

        public string GetContext(LiquidExpressionContext expressionContext) {
            var path = expressionContext.FilePath.TrimStart(this.BasePath);
            return path.Replace('\\', '.').Replace(".liquid", string.Empty);
        }

        public LocalizableStringLocation GetLocation(LiquidExpressionContext expressionContext) {
            return new LocalizableStringLocation() {
                SourceFile = expressionContext.FilePath.TrimStart(this.BasePath)
            };
        }
    }
}
