using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Users
{
    public class UsersOutputDto : IMapFrom<Entities.Models.User>
    {
        public int Id { get; set; }

        public string StudentId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }

    }
}
