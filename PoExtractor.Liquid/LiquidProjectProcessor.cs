using Fluid;
using Fluid.Parser;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Liquid;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.Liquid.MetadataProviders;
using System.IO;
using System.Linq;

namespace PoExtractor.Liquid {
    /// <summary>
    /// Extracts localizable strings from all *.liquid files in the project path
    /// </summary>
    public class LiquidProjectProcessor : IProjectProcessor {
        private readonly LiquidViewParser _parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiquidProjectProcessor"/>
        /// </summary>
        public LiquidProjectProcessor() {
            var parserOptions = Options.Create(new LiquidViewOptions());
            _parser = new LiquidViewParser(parserOptions);
        }

        public void Process(string path, string basePath, LocalizableStringCollection strings) {
            var liquidMetadataProvider = new LiquidMetadataProvider(basePath);
            var liquidVisitor = new ExtractingLiquidWalker(new[] { new LiquidStringExtractor(liquidMetadataProvider) }, strings);
            
            foreach (var file in Directory.EnumerateFiles(path, "*.liquid", SearchOption.AllDirectories).OrderBy(file => file)) {
                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {
                        if (_parser.TryParse(reader.ReadToEnd(), out var template, out var errors)) {
                            ProcessTemplate(template, liquidVisitor, file);
                        }
                    }
                }
            }
        }

        private void ProcessTemplate(IFluidTemplate template, ExtractingLiquidWalker visitor, string path)
        {
            if (template is CompositeFluidTemplate compositeTemplate)
            {
                foreach (var innerTemplate in compositeTemplate.Templates)
                {
                    ProcessTemplate(innerTemplate, visitor, path);
                }
            }
            else if (template is FluidTemplate singleTemplate)
            {
                ProcessTemplate(singleTemplate, visitor, path);
            }
        }

        private void ProcessTemplate(FluidTemplate template, ExtractingLiquidWalker visitor, string path)
        {
            foreach (var statement in template.Statements)
            {
                visitor.Visit(new LiquidStatementContext() { Statement = statement, FilePath = path });
            }
        }
    }
}
