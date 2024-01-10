using System.ComponentModel.DataAnnotations;

namespace IfRolesExample.ViewModels
{
    public class UserVM
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
