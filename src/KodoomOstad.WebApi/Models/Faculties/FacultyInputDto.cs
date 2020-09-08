using System.ComponentModel.DataAnnotations;
using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Faculties
{
    public class FacultyInputDto : IMapTo<Faculty>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }
    }
}
