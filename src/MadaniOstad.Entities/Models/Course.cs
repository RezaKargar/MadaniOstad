using MadaniOstad.Entities.Models.Base;

namespace MadaniOstad.Entities.Models
{
    public class Course : AuditableBaseEntity
    {
        public string Title { get; set; }

        public decimal Grade { get; set; }

        public int Year { get; set; }

        public int Term { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }
    }
}
