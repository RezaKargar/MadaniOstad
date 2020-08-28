using KodoomOstad.Entities.Models.Base;
using System.Collections.Generic;

namespace KodoomOstad.Entities.Models
{
    public class User : AuditableBaseEntity
    {
        public string Name { get; set; }

        public string StudentId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Course> Courses { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
