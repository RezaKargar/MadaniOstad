using System.ComponentModel.DataAnnotations;

namespace KodoomOstad.WebApi.Models.Users
{
    public class UsersChangePasswordInputDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "New password and its repeat doesn't match.")]
        public string NewPasswordRepeat { get; set; }
    }
}
