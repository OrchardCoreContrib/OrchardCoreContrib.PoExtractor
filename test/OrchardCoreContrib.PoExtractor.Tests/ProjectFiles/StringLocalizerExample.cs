using OrchardCoreContrib.PoExtractor.Tests.Fakes;

namespace OrchardCoreContrib.PoExtractor.Tests.ProjectFiles
{
    public class StringLocalizerExample
    {
        private readonly FakeStringLocalizer S = new();

        public void Singular()
        {
            _ = S["Thing"];
        }

        public void Plural()
        {
            S.Plural(1, "{0} thing", "{0} things");
        }
    }
}
