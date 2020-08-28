using KodoomOstad.Entities.Models.Base;
using System.Collections.Generic;

namespace KodoomOstad.Entities.Models
{
    public class PollQuestion : AuditableBaseEntity
    {
        public string Question { get; set; }

        public int MinScore { get; set; }

        public int MaxScore { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
