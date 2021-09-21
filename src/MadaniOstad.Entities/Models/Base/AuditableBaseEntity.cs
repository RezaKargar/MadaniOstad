using System;

namespace MadaniOstad.Entities.Models.Base
{
    public interface IAuditableBaseEntity : IEntity
    {
        DateTime CreatedAt { get; set; }

        DateTime? LastModifiedAt { get; set; }
    }

    public abstract class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditableBaseEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }

    public abstract class AuditableBaseEntity : AuditableBaseEntity<int>
    {
    }
}
