using System.ComponentModel.DataAnnotations;
using MadaniOstad.Entities.Models;
using MadaniOstad.IocConfig.CustomMapping;

namespace MadaniOstad.WebApi.Models.Faculties
{
    public class FacultyInputDto : IMapTo<Faculty>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }
    }
}
