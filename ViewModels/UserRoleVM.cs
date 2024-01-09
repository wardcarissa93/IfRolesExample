using System.ComponentModel.DataAnnotations;

namespace IfRolesExample.ViewModels
{
    public class UserRoleVM
    {
        [Display(Name = "ID")]
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
