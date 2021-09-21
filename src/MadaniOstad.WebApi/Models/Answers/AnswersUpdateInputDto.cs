using MadaniOstad.Entities.Models;
using MadaniOstad.IocConfig.CustomMapping;

namespace MadaniOstad.WebApi.Models.Answers
{
    public class AnswersUpdateInputDto : IMapTo<Answer>
    {
        public int Score { get; set; }
    }
}
