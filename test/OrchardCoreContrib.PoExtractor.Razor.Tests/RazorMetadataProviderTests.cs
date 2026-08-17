using Xunit;
using System;
using System.IO;
using System.Reflection;

namespace OrchardCoreContrib.PoExtractor.Razor.Tests
{
    public class RazorMetadataProviderTests
    {
        [Theory]
        [InlineData("/MyProject/Views/Home/Index.cshtml", "Views/Home/Index.cshtml")]
        [InlineData("\\MyProject\\Views\\Home\\Index.cshtml", "Views/Home/Index.cshtml")]
        [InlineData("MyProject/Index.cshtml", "Index.cshtml")]
        [InlineData("Index.cshtml", "Index.cshtml")]
        public void RemoveProjectNameFromPath_RemovesProjectName(string input, string expected)
        {
            // Use reflection to access the private static method
            var type = typeof(OrchardCoreContrib.PoExtractor.Razor.MetadataProviders.RazorMetadataProvider);
            var method = type.GetMethod("RemoveProjectNameFromPath", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Normalize input for current OS
            var normalizedInput = input.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            var normalizedExpected = expected.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            var result = (string)method.Invoke(null, new object[] { normalizedInput });
            Assert.Equal(normalizedExpected, result);
        }
    }
}
