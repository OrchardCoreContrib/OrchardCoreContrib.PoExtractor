using Fluid;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.Liquid.MetadataProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PoExtractor.Liquid {
    /// <summary>
    /// Extracts localizable strings from all *.liquid files in the project path
    /// </summary>
    public class LiquidProjectProcessor : IProjectProcessor {
        private readonly IFluidParser _parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiquidProjectProcessor"/>
        /// </summary>
        /// <param name="configurationAction">the action used to configure <see cref="FluidParserFactory"/>. Custom filters should be registered in the configuration action.</param>
        public LiquidProjectProcessor(Action<FluidParserFactory> configurationAction) {
            var parserFactory = new FluidParserFactory();
            configurationAction?.Invoke(parserFactory);

            _parser = parserFactory.CreateParser();
        }

        public void Process(string path, string basePath, LocalizableStringCollection strings) {
            var liquidMetadataProvider = new LiquidMetadataProvider(basePath);
            var liquidVisitor = new ExtractingLiquidWalker(new[] { new LiquidStringExtractor(liquidMetadataProvider) }, strings);
            
            foreach (var file in Directory.EnumerateFiles(path, "*.liquid", SearchOption.AllDirectories)) {
                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {

                        if (_parser.TryParse(reader.ReadToEnd(), true, out var ast, out var errors)) {
                            foreach (var statement in ast) {
                                liquidVisitor.Visit(new LiquidStatementContext() { Statement = statement, FilePath = file });
                            }
                        }
                    }
                }
            }
        }
    }
}
