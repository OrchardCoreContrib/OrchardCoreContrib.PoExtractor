using System.ComponentModel.DataAnnotations;

namespace PoExtractor.Core.Tests.ProjectFiles
{
    public class PersonModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Range(15, 45, ErrorMessage = "Age should be in the range [15-45].")]
        public int Age { get; set; }
    }
}
