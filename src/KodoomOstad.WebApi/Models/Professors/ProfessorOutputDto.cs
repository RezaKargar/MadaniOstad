using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Professors
{
    public class ProfessorOutputDto : IMapFrom<Professor>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }

        public int FacultyId { get; set; }
    }
}
