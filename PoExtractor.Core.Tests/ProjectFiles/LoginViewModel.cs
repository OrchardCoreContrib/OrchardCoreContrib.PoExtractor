using System.ComponentModel.DataAnnotations;

namespace PoExtractor.Core.Tests.Files
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The username is required.")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
