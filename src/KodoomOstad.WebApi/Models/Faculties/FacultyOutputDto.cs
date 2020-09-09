using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Faculties
{
    public class FacultyOutputDto : IMapFrom<Faculty>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }
    }
}
