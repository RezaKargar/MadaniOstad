using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Courses
{
    public class CoursesOutputDto : IMapFrom<Course>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public decimal Grade { get; set; }

        public int Year { get; set; }

        public int Term { get; set; }

        public int UserId { get; set; }

        public int ProfessorId { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }
    }
}
