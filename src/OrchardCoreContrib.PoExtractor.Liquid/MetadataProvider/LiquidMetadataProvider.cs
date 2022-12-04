using System.IO;

namespace OrchardCoreContrib.PoExtractor.Liquid.MetadataProviders
{
    /// <summary>
    /// Provides metadata for .liquid files
    /// </summary>
    public class LiquidMetadataProvider : IMetadataProvider<LiquidExpressionContext>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="LiquidMetadataProvider"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        public LiquidMetadataProvider(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Gets the base path.
        /// </summary>
        public string BasePath { get; set; }

        /// <inheritdoc/>
        public string GetContext(LiquidExpressionContext expressionContext)
        {
            var path = expressionContext.FilePath.TrimStart(this.BasePath);
            return path.Replace(Path.DirectorySeparatorChar, '.').Replace(".liquid", string.Empty);
        }

        /// <inheritdoc/>
        public LocalizableStringLocation GetLocation(LiquidExpressionContext expressionContext)
        {
            return new LocalizableStringLocation()
            {
                SourceFile = expressionContext.FilePath.TrimStart(BasePath)
            };
        }
    }
}
