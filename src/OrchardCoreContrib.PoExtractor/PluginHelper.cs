using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using OrchardCoreContrib.PoExtractor.DotNet;
using OrchardCoreContrib.PoExtractor.DotNet.CS;
using OrchardCoreContrib.PoExtractor.DotNet.VB;
using OrchardCoreContrib.PoExtractor.Liquid;
using OrchardCoreContrib.PoExtractor.Razor;

namespace OrchardCoreContrib.PoExtractor;

public static class PluginHelper
{
    public static async Task ProcessPluginsAsync(
        IEnumerable<string> plugins,
        List<IProjectProcessor> projectProcessors,
        List<string> projectFiles,
        IEnumerable<Assembly> assemblies = null)
    {
        assemblies ??=
        [
            typeof(IProjectProcessor).Assembly, // OrchardCoreContrib.PoExtractor.Abstractions
            typeof(ExtractingCodeWalker).Assembly, // OrchardCoreContrib.PoExtractor.DotNet
            typeof(CSharpProjectProcessor).Assembly, // OrchardCoreContrib.PoExtractor.DotNet.CS
            typeof(VisualBasicProjectProcessor).Assembly, // OrchardCoreContrib.PoExtractor.DotNet.VB
            typeof(LiquidProjectProcessor).Assembly, // OrchardCoreContrib.PoExtractor.Liquid
            typeof(RazorProjectProcessor).Assembly, // OrchardCoreContrib.PoExtractor.Razor
        ];
        
        var sharedOptions = ScriptOptions.Default.AddReferences(assemblies);

        foreach (var plugin in plugins)
        {
            string code;
            ScriptOptions options;

            if (plugin.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                code = await new HttpClient().GetStringAsync(plugin);
                options = sharedOptions.WithFilePath(Path.Join(
                    Environment.CurrentDirectory,
                    Path.GetFileName(new Uri(plugin).AbsolutePath)));
            }
            else
            {
                code = await File.ReadAllTextAsync(plugin);
                options = sharedOptions.WithFilePath(Path.GetFullPath(plugin));
            }

            await CSharpScript.EvaluateAsync(code, options, new PluginContext(projectProcessors, projectFiles));
        }
    }

    public record PluginContext(List<IProjectProcessor> ProjectProcessors, List<string> ProjectFiles);
}
