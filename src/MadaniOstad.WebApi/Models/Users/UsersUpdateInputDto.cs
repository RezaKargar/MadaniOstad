using MadaniOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace MadaniOstad.WebApi.Models.Users
{
    public class UsersUpdateInputDto : IMapTo<Entities.Models.User>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        public string Name { get; set; }

        public string StudentId { get; set; }
    }
}
