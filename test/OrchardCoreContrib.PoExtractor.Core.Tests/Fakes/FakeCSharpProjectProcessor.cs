using OrchardCoreContrib.PoExtractor.DotNet.CS;

namespace OrchardCoreContrib.PoExtractor.Tests.Fakes;

public class FakeCSharpProjectProcessor : IProjectProcessor
{
    private static readonly string _defaultPath = "ProjectFiles";

    public void Process(string path, string basePath, LocalizableStringCollection localizableStrings)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = _defaultPath;
        }

        if (string.IsNullOrEmpty(basePath))
        {
            basePath = _defaultPath;
        }

        var csharpProjectProcessor = new CSharpProjectProcessor();

        csharpProjectProcessor.Process(path, basePath, localizableStrings);
    }
}
