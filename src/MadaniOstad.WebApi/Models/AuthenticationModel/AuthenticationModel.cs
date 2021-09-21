using System.ComponentModel.DataAnnotations;

namespace MadaniOstad.WebApi.Models.AuthenticationModel
{
    public class AuthenticationModel
    {
        [Required]
        public string Grant_Type { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
