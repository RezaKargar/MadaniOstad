using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Answers
{
    public class AnswersInputDto : IMapTo<Answer>
    {
        public int Score { get; set; }

        public int ProfessorId { get; set; }

        public int PollQuestionId { get; set; }

        public int UserId { get; set; }
    }
}
