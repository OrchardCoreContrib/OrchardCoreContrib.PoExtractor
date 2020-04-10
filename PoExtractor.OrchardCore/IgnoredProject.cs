namespace PoExtractor.OrchardCore
{
    public class IgnoredProject
    {
        public static readonly string Cms = "src\\OrchardCore.Cms.Web";

        public static readonly string Samples = "src\\OrchardCore.Mvc.Web";

        public static readonly string Test = "test";

        public static string[] ToList() => new[] { Cms, Samples, Test };
    }
}
