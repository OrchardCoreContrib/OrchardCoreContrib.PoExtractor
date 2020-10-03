using System.ComponentModel.DataAnnotations;

namespace PoExtractor.Core.Tests.ProjectFiles
{
    public class PersonModel
    {
        [Display(Name = "First name", ShortName = "1st name", Description = "The first name of the person", GroupName = "Person info")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Range(15, 45, ErrorMessage = "Age should be in the range [15-45].")]
        public int Age { get; set; }
    }
}
