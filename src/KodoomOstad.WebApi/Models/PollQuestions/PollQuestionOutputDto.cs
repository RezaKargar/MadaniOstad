using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.PollQuestions
{
    public class PollQuestionOutputDto : IMapFrom<PollQuestion>
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public int MinScore { get; set; }

        public int MaxScore { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }
    }
}
