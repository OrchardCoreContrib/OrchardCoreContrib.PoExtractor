using System.Collections.Generic;

namespace OrchardCoreContrib.PoExtractor;

public class IgnoredProject
{
    public static readonly string Docs = "src\\docs";

    public static readonly string Cms = "src\\OrchardCore.Cms.Web";

    public static readonly string Mvc = "src\\OrchardCore.Mvc.Web";

    public static readonly string Templates = "src\\Templates";

    public static readonly string Test = "test";

    private static readonly List<string> _ignoredProjects = [ Docs, Cms, Mvc, Templates ];

    public static void Add(string project) => _ignoredProjects.Add(project);

    public static IEnumerable<string> ToList() => _ignoredProjects;
}
