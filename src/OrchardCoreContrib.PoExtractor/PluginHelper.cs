using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace OrchardCoreContrib.PoExtractor;

public static class PluginHelper
{
    public static async Task ProcessPluginsAsync(
        IEnumerable<string> plugins,
        List<IProjectProcessor> projectProcessors,
        List<string> projectFiles)
    {
        var sharedOptions = ScriptOptions.Default.AddReferences(typeof(Program).Assembly);

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
