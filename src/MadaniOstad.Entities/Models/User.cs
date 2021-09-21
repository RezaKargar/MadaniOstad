using MadaniOstad.Entities.Models.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MadaniOstad.Entities.Models
{
    public class User : IdentityUser<int>, IAuditableBaseEntity
    {
        public string Name { get; set; }

        public string StudentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Course> Courses { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
