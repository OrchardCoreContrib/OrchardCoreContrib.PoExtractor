using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace OrchardCoreContrib.PoExtractor.Tests.ProjectFiles
{
    public class StringLocalizerExample
    {
        private readonly IStringLocalizer S;

        public void Singular()
        {
            _ = S["Thing"];
        }

        public void Plural()
        {
            _ = S.Plural(1, "{0} thing", "{0} things");
        }
    }
}
