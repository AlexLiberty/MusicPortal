using Resources;
using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models.ViewModel
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "RequiredEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "RequiredName")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "RequiredPassword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "RequiredConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
    }
}
