using Fluid;
using Fluid.Parser;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCoreContrib.PoExtractor.Liquid.MetadataProviders;
using System;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.Liquid;

/// <summary>
/// Extracts localizable strings from all *.liquid files in the project path
/// </summary>
public class LiquidProjectProcessor : IProjectProcessor
{
    private static readonly string _liquidExtension = "*.liquid";

    private readonly LiquidViewParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiquidProjectProcessor"/>
    /// </summary>
    public LiquidProjectProcessor()
    {
        var parserOptions = Options.Create(new LiquidViewOptions());

        _parser = new LiquidViewParser(parserOptions);
    }

    /// <inheritdoc/>
    public void Process(string path, string basePath, LocalizableStringCollection localizableStrings)
    {
        ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
        ArgumentException.ThrowIfNullOrEmpty(basePath, nameof(basePath));
        ArgumentNullException.ThrowIfNull(localizableStrings);

        var liquidMetadataProvider = new LiquidMetadataProvider(basePath);
        var liquidVisitor = new ExtractingLiquidWalker([new LiquidStringExtractor(liquidMetadataProvider)], localizableStrings);

        foreach (var file in Directory.EnumerateFiles(path, $"*{_liquidExtension}", SearchOption.AllDirectories).OrderBy(file => file))
        {
            using var stream = File.OpenRead(file);
            using var reader = new StreamReader(stream);
            if (_parser.TryParse(reader.ReadToEnd(), out var template, out var errors))
            {
                ProcessTemplate(template, liquidVisitor, file);
            }
        }
    }

    private static void ProcessTemplate(IFluidTemplate template, ExtractingLiquidWalker visitor, string path)
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

    private static void ProcessTemplate(FluidTemplate template, ExtractingLiquidWalker visitor, string path)
    {
        foreach (var statement in template.Statements)
        {
            visitor.Visit(new LiquidStatementContext() { Statement = statement, FilePath = path });
        }
    }
}
