using Resources;
using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "RequiredEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "RequiredPassword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
