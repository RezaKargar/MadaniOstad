using System;

namespace KodoomOstad.Entities.Models.Base
{
    public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }

    public abstract class AuditableBaseEntity : AuditableBaseEntity<int>
    {
    }
}
