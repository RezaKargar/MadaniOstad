using System.ComponentModel.DataAnnotations;

namespace KodoomOstad.Entities.Models.Base
{
    public interface IEntity
    {
    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        [Key]
        public TKey Id { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}
