using Microsoft.CodeAnalysis.VisualBasic;
using OrchardCoreContrib.PoExtractor.DotNet.VB.MetadataProviders;
using System;
using System.IO;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB;

/// <summary>
/// Extracts localizable strings from all *.vb files in the project path.
/// </summary>
public class VisualBasicProjectProcessor : IProjectProcessor
{
    private static readonly string _visualBasicExtension = "*.vb";

    /// <inheritdoc/>
    public void Process(string path, string basePath, LocalizableStringCollection localizableStrings)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
        }

        if (string.IsNullOrEmpty(basePath))
        {
            throw new ArgumentException($"'{nameof(basePath)}' cannot be null or empty.", nameof(basePath));
        }

        ArgumentNullException.ThrowIfNull(localizableStrings);

        var visualBasicMetadataProvider = new VisualBasicMetadataProvider(basePath);
        var visualBasicWalker = new ExtractingCodeWalker(
        [
            new SingularStringExtractor(visualBasicMetadataProvider),
            new PluralStringExtractor(visualBasicMetadataProvider),
            new ErrorMessageAnnotationStringExtractor(visualBasicMetadataProvider),
            new DisplayAttributeDescriptionStringExtractor(visualBasicMetadataProvider),
            new DisplayAttributeNameStringExtractor(visualBasicMetadataProvider),
            new DisplayAttributeGroupNameStringExtractor(visualBasicMetadataProvider),
            new DisplayAttributeShortNameStringExtractor(visualBasicMetadataProvider)
        ], localizableStrings);

        foreach (var file in Directory.EnumerateFiles(path, $"*{_visualBasicExtension}", SearchOption.AllDirectories).OrderBy(file => file))
        {
            using var stream = File.OpenRead(file);
            using var reader = new StreamReader(stream);
            var syntaxTree = VisualBasicSyntaxTree.ParseText(reader.ReadToEnd(), path: file);

            visualBasicWalker.Visit(syntaxTree.GetRoot());
        }
    }
}
