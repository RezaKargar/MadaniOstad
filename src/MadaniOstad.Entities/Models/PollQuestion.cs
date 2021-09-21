using MadaniOstad.Entities.Models.Base;
using System.Collections.Generic;

namespace MadaniOstad.Entities.Models
{
    public class PollQuestion : AuditableBaseEntity
    {
        public string Question { get; set; }

        public int MinScore { get; set; }

        public int MaxScore { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
