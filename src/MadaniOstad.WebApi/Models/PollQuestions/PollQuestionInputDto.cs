using MadaniOstad.Entities.Models;
using MadaniOstad.IocConfig.CustomMapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MadaniOstad.WebApi.Models.PollQuestions
{
    public class PollQuestionInputDto : IMapTo<PollQuestion>, IValidatableObject
    {
        [Required]
        public string Question { get; set; }

        [Required]
        public int MinScore { get; set; }

        [Required]
        public int MaxScore { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinScore == MaxScore)
                yield return new ValidationResult("MinScore and MaxScore can not have same value.", new[] { nameof(MinScore), nameof(MaxScore) });
        }
    }
}
