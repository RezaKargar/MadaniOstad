using KodoomOstad.Entities.Models.Base;
using System.Collections.Generic;

namespace KodoomOstad.Entities.Models
{
    public class Professor : AuditableBaseEntity
    {
        public string Name { get; set; }

        public string Avatar { get; set; }

        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public ICollection<Comment> Collection { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
