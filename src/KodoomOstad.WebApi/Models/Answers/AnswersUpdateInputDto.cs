using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Answers
{
    public class AnswersUpdateInputDto : IMapTo<Answer>
    {
        public int Score { get; set; }
    }
}
