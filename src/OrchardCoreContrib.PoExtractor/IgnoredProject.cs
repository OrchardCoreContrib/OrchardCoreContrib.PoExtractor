using System.Collections.Generic;

namespace OrchardCoreContrib.PoExtractor
{
    public class IgnoredProject
    {
        public static readonly string Docs = "src\\dos";

        public static readonly string Cms = "src\\OrchardCore.Cms.Web";

        public static readonly string Mvc = "src\\OrchardCore.Mvc.Web";

        public static readonly string Templates = "src\\Templates";

        public static readonly string Test = "test";

        public static IEnumerable<string> ToList()
        {
            yield return Docs;
            yield return Cms;
            yield return Mvc;
            yield return Templates;
            yield return Test;
        }
    }
}
