using System.ComponentModel.DataAnnotations;

namespace IfRolesExample.ViewModels
{
    public class UserVM
    {
        [Key]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
