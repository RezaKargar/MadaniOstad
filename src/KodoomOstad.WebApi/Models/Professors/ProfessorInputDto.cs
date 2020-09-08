using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace KodoomOstad.WebApi.Models.Professors
{
    public class ProfessorInputDto : IMapTo<Professor>
    {
        [Required]
        public string Name { get; set; }

        public string Avatar { get; set; }

        [Required]
        public int FacultyId { get; set; }
    }
}
