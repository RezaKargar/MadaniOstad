using MadaniOstad.Entities.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MadaniOstad.Entities.Models
{
    public class Professor : AuditableBaseEntity
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public string Avatar { get; set; }

        public ProfessorRank Rank { get; set; }

        public float AverageRate { get; set; }

        public bool IsFacultyMember { get; set; }

        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Course> Courses { get; set; }
    }

    public enum ProfessorRank
    {

        [Display(Name = "مربی آموزشیار")]
        AssistantInstructor,

        [Display(Name = "مربی")]
        Instructor,

        [Display(Name = "مدرس")]
        Lecturer,

        [Display(Name = "استادیار")]
        AssistantProfessor,

        [Display(Name = "دانشیار")]
        AssociateProfessor,

        [Display(Name = "استاد تمام")]
        FullProfessor,

        [Display(Name = "استاد ممتاز")]
        DistinguishedProfessor
    }
}
