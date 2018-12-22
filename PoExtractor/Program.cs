using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using OrchardCore.DisplayManagement.Liquid.Tags;
using OrchardCore.DynamicCache.Liquid;
using PoExtractor.Core.Extractors;
using PoExtractor.Core.MetadataProviders;
using PoExtractor.Extractors;
using PoExtractor.MetadataProviders;
using F = Fluid;

namespace PoExtractor.Core {
    class Program {
        private static Func<string, string, string, string> DefaultOutputPathGenerator = (string basePath, string outputBasePath, string projectFilePath) => {
            var projectPath = Path.GetDirectoryName(projectFilePath);
            return Path.Combine(outputBasePath, projectPath.TrimStart(basePath + Path.DirectorySeparatorChar) + ".pot");
        };

        private static Func<string, string, string, string> OrchardOutputPathGenerator = (string basePath, string outputBasePath, string projectFilePath) => {
            var projectRelativePath = projectFilePath.TrimStart(Path.Combine(basePath, "src" + Path.DirectorySeparatorChar));
            var projectDir = projectRelativePath.Split(Path.DirectorySeparatorChar).First();

            var projectPath = Path.GetDirectoryName(projectFilePath);


            return Path.Combine(outputBasePath, projectDir, Path.GetFileNameWithoutExtension(projectFilePath) + ".pot");
        };

        private static F.FluidParserFactory _liquidParseFactory;
        private static string[] ProjectBlacklist = new[] { "test", "src\\OrchardCore.Cms.Web", "src\\OrchardCore.Mvc.Web", "src\\OrchardCore.Nancy.Web" };

        static Program() {
            _liquidParseFactory = new F.FluidParserFactory();
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

            // Dynamic caching
            _liquidParseFactory.RegisterBlock<CacheBlock>("cache");
            _liquidParseFactory.RegisterTag<CacheDependencyTag>("cache_dependency");
            _liquidParseFactory.RegisterTag<CacheExpiresOnTag>("cache_expires_on");
            _liquidParseFactory.RegisterTag<CacheExpiresAfterTag>("cache_expires_after");
            _liquidParseFactory.RegisterTag<CacheExpiresSlidingTag>("cache_expires_sliding");
        }

        static void Main(string[] args) {
            if (args.Length != 2) {
                WriteHelp();
                return;
            }

            var basePath = args[0];
            var outputBasePath = args[1];

            string[] projectFiles;
            if (Directory.Exists(basePath)) {
                projectFiles = Directory.EnumerateFiles(basePath, "*.csproj", SearchOption.AllDirectories).ToArray();
            } else {
                WriteHelp();
                return;
            }

            foreach (var projectFilePath in projectFiles) {
                var projectPath = Path.GetDirectoryName(projectFilePath);
                var projectBasePath = Path.GetDirectoryName(projectPath) + Path.DirectorySeparatorChar;
                var projectRelativePath = projectPath.TrimStart(basePath + Path.DirectorySeparatorChar);

                if (ProjectBlacklist.Any(o => projectRelativePath.StartsWith(o))) {
                    continue;
                }

                ProcessProject(projectPath, projectBasePath, OrchardOutputPathGenerator(args[0], args[1], projectFilePath));
            }
        }

        private static void WriteHelp() {
            Console.WriteLine("Usage: PoExtractor.exe input output");
            Console.WriteLine("    input: Path to input directory, all projects at the the path will be processed");
            Console.WriteLine("    output: Path to a directory where POT files will be generated");
        }

        private static void ProcessProject(string projectPath, string projectBasePath, string outputFilePath) {
            var strings = new LocalizableStringCollection();

            var codeMetadataProvider = new CodeMetadataProvider(projectBasePath);
            var localizedStringsCollector = new CSharpCodeVisitor(
                new IStringExtractor<SyntaxNode>[] {
                    new SingularStringExtractor(codeMetadataProvider),
                    new PluralStringExtractor(codeMetadataProvider)
                }, strings);

            foreach (var file in Directory.EnumerateFiles(projectPath, "*.cs", SearchOption.AllDirectories)) {
                if (Path.GetFileName(file).EndsWith(".g.cshtml.cs")) {
                    continue;
                }

                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {
                        var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

                        localizedStringsCollector.Visit(syntaxTree.GetRoot());
                    }
                }
            }

            var liquidMetadataProvider = new LiquidMetadataProvider(projectBasePath);
            var liquidVisitor = new LiquidVisitor(new[] { new LiquidStringExtractor(liquidMetadataProvider) }, strings);
            var liquidParser = _liquidParseFactory.CreateParser();

            foreach (var file in Directory.EnumerateFiles(projectPath, "*.liquid", SearchOption.AllDirectories)) {
                using (var stream = File.OpenRead(file)) {
                    using (var reader = new StreamReader(stream)) {

                        if (liquidParser.TryParse(reader.ReadToEnd(), out var ast, out var errors)) {
                            foreach (var statement in ast) {
                                liquidVisitor.Visit(new LiquidStatementContext() { Statement = statement, FilePath = file });
                            }
                        }
                    }
                }
            }

            var razorMetadataProvider = new RazorMetadataProvider(projectBasePath);
            localizedStringsCollector.Extractors = new IStringExtractor<SyntaxNode>[] {
                new SingularStringExtractor(razorMetadataProvider),
                new PluralStringExtractor(razorMetadataProvider)
            };

            var compiledViews = ViewCompiler.CompileViews(projectPath);
            var cs = new CSharpCodeProvider();

            foreach (var view in compiledViews) {
                try {
                    var syntaxTree = CSharpSyntaxTree.ParseText(view.GeneratedCode, path: view.FilePath);
                    localizedStringsCollector.Visit(syntaxTree.GetRoot());
                } catch (Exception ex) {
                    Console.WriteLine("Compile fail for: {0}", view.FilePath);
                }
            }

            if (strings.Values.Any()) {
                var outputDirectory = Path.GetDirectoryName(outputFilePath);
                Directory.CreateDirectory(outputDirectory);

                using (var potFile = new PotFile(outputFilePath)) {
                    potFile.WriteRecord(strings.Values);
                }
            }

            Console.WriteLine($"{Path.GetFileName(projectPath)}: Found {strings.Values.Count()} strings.");
        }
    }
}
