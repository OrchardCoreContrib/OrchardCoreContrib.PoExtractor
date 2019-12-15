using Fluid;
using OrchardCore.DisplayManagement.Liquid.Tags;
using OrchardCore.DynamicCache.Liquid;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using PoExtractor.CS;
using PoExtractor.Liquid;
using PoExtractor.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PoExtractor.OrchardCore {
    class Program {
        private static string[] ProjectBlacklist = new[] { "test", "src\\OrchardCore.Cms.Web", "src\\OrchardCore.Mvc.Web", "src\\OrchardCore.Nancy.Web" };

        static void Main(string[] args) {
            if (args.Length < 2) {
                WriteHelp();
                return;
            }

            var basePath = args[0];
            var outputBasePath = args[1];
            var parseLiquid = args.Length > 2 && args[2] == "--liquid";

            string[] projectFiles;
            if (Directory.Exists(basePath)) {
                projectFiles = Directory.EnumerateFiles(basePath, "*.csproj", SearchOption.AllDirectories)
                    .Union(Directory.EnumerateFiles(basePath, "*.vbproj", SearchOption.AllDirectories)).ToArray();
            } else {
                WriteHelp();
                return;
            }

            var processors = new List<IProjectProcessor>
            {
                new CSharpProjectProcessor(),
                new VisualBasicProjectProcessor()
            };

            if (parseLiquid) {
                processors.Add(new LiquidProjectProcessor(ConfigureFluidParser));
            };

            foreach (var projectFilePath in projectFiles) {
                var projectPath = Path.GetDirectoryName(projectFilePath);
                var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var projectRelativePath = projectPath.TrimStart(basePath + Path.DirectorySeparatorChar);
                var outputPath = Path.Combine(outputBasePath, Path.GetFileNameWithoutExtension(projectFilePath) + ".pot");

                if (ProjectBlacklist.Any(o => projectRelativePath.StartsWith(o))) {
                    continue;
                }

                var strings = new LocalizableStringCollection();
                foreach (var processor in processors) {
                    processor.Process(projectPath, projectBasePath, strings);
                }

                if (strings.Values.Any()) {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                    using (var potFile = new PoWriter(outputPath)) {
                        potFile.WriteRecord(strings.Values);
                    }
                }

                Console.WriteLine($"{Path.GetFileName(projectPath)}: Found {strings.Values.Count()} strings.");
            }
        }

        private static void WriteHelp() {
            Console.WriteLine("Usage: extractpo-oc input output --liquid");
            Console.WriteLine("    input: path to the input directory, all projects at the the path will be processed");
            Console.WriteLine("    output: path to a directory where POT files will be generated");
            Console.WriteLine("    --liquid: include this flag to process .liquid files");
        }

        private static void ConfigureFluidParser(FluidParserFactory _liquidParseFactory) {
            _liquidParseFactory.RegisterTag<RenderBodyTag>("render_body");
            _liquidParseFactory.RegisterTag<RenderSectionTag>("render_section");
            _liquidParseFactory.RegisterTag<RenderTitleSegmentsTag>("page_title");
            _liquidParseFactory.RegisterTag<AntiForgeryTokenTag>("antiforgerytoken");
            _liquidParseFactory.RegisterTag<LayoutTag>("layout");

            _liquidParseFactory.RegisterTag<ClearAlternatesTag>("shape_clear_alternates");
            _liquidParseFactory.RegisterTag<AddAlternatesTag>("shape_add_alternates");
            _liquidParseFactory.RegisterTag<ClearWrappers>("shape_clear_wrappers");
            _liquidParseFactory.RegisterTag<AddWrappersTag>("shape_add_wrappers");
            _liquidParseFactory.RegisterTag<ClearClassesTag>("shape_clear_classes");
            _liquidParseFactory.RegisterTag<AddClassesTag>("shape_add_classes");
            _liquidParseFactory.RegisterTag<ClearAttributesTag>("shape_clear_attributes");
            _liquidParseFactory.RegisterTag<AddAttributesTag>("shape_add_attributes");
            _liquidParseFactory.RegisterTag<ShapeTypeTag>("shape_type");
            _liquidParseFactory.RegisterTag<ShapeDisplayTypeTag>("shape_display_type");
            _liquidParseFactory.RegisterTag<ShapePositionTag>("shape_position");
            _liquidParseFactory.RegisterTag<ShapeTabTag>("shape_tab");
            _liquidParseFactory.RegisterTag<ShapeRemoveItemTag>("shape_remove_item");
            _liquidParseFactory.RegisterTag<ShapePagerTag>("shape_pager");

            _liquidParseFactory.RegisterTag<HelperTag>("helper");
            _liquidParseFactory.RegisterTag<NamedHelperTag>("shape");
            _liquidParseFactory.RegisterTag<NamedHelperTag>("link");
            _liquidParseFactory.RegisterTag<NamedHelperTag>("meta");
            _liquidParseFactory.RegisterTag<NamedHelperTag>("resources");
            _liquidParseFactory.RegisterTag<NamedHelperTag>("script");
            _liquidParseFactory.RegisterTag<NamedHelperTag>("style");

            _liquidParseFactory.RegisterBlock<HelperBlock>("block");
            _liquidParseFactory.RegisterBlock<NamedHelperBlock>("a");
            _liquidParseFactory.RegisterBlock<NamedHelperBlock>("zone");
            _liquidParseFactory.RegisterBlock<NamedHelperBlock>("scriptblock");

            _liquidParseFactory.RegisterBlock<CacheBlock>("cache");
            _liquidParseFactory.RegisterTag<CacheDependencyTag>("cache_dependency");
            _liquidParseFactory.RegisterTag<CacheExpiresOnTag>("cache_expires_on");
            _liquidParseFactory.RegisterTag<CacheExpiresAfterTag>("cache_expires_after");
            _liquidParseFactory.RegisterTag<CacheExpiresSlidingTag>("cache_expires_sliding");
        }
    }
}
