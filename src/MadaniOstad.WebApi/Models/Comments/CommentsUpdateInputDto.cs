using MadaniOstad.Entities.Models;
using MadaniOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace MadaniOstad.WebApi.Models.Comments
{
    public class CommentsUpdateInputDto : IMapTo<Comment>
    {
        [Required]
        public string Text { get; set; }
    }
}
