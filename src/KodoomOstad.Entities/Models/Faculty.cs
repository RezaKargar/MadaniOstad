using KodoomOstad.Entities.Models.Base;
using System.Collections.Generic;

namespace KodoomOstad.Entities.Models
{
    public class Faculty : AuditableBaseEntity
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public ICollection<Professor> Professors { get; set; }
    }
}
