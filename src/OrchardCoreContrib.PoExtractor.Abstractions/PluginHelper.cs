using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace OrchardCoreContrib.PoExtractor;

public static class PluginHelper
{
    public static async Task ProcessPluginsAsync(
        IList<string> plugins,
        List<IProjectProcessor> projectProcessors,
        List<string> projectFiles,
        IEnumerable<Assembly> assemblies)
    {
        var options = ScriptOptions.Default.AddReferences(assemblies);

        foreach (var plugin in plugins)
        {
            var code = await File.ReadAllTextAsync(plugin);
            await CSharpScript.EvaluateAsync(code, options, new PluginContext(projectProcessors, projectFiles));
        }
    }

    public record PluginContext(List<IProjectProcessor> projectProcessors, List<string> projectFiles);
}