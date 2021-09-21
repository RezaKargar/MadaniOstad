using MadaniOstad.Entities.Models;
using MadaniOstad.IocConfig.CustomMapping;

namespace MadaniOstad.WebApi.Models.Answers
{
    public class AnswersInputDto : IMapTo<Answer>
    {
        public int Score { get; set; }

        public int ProfessorId { get; set; }

        public int PollQuestionId { get; set; }

        public int UserId { get; set; }
    }
}
