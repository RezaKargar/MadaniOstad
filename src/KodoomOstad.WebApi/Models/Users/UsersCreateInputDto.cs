using KodoomOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace KodoomOstad.WebApi.Models.Users
{
    public class UsersCreateInputDto : IMapTo<Entities.Models.User>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "{0} must be at least 6 characters")]
        public string Password { get; set; }

        public string StudentId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }
    }
}
