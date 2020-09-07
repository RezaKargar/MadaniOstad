﻿using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Answers
{
    public class AnswersOutputDto : IMapFrom<Answer>
    {
        public int Id { get; set; }

        public int Score { get; set; }

        public int ProfessorId { get; set; }

        public int PollQuestionId { get; set; }

        public int UserId { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }
    }
}
