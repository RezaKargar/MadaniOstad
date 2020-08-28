using KodoomOstad.Entities.Models.Base;
using System.Collections.Generic;

namespace KodoomOstad.Entities.Models
{
    public class Comment : AuditableBaseEntity
    {
        public string Text { get; set; }

        public bool IsDeleted { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? ReplyToId { get; set; }
        public Comment ReplyTo { get; set; }

        public ICollection<Comment> Replies { get; set; }
    }
}
