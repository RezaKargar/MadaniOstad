using KodoomOstad.Entities.Models.Base;

namespace KodoomOstad.Entities.Models
{
    public class Answer : AuditableBaseEntity
    {
        public int Score { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }

        public int PollQuestionId { get; set; }
        public PollQuestion PollQuestion { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
