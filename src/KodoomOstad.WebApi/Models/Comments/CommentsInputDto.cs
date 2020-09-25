using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace KodoomOstad.WebApi.Models.Comments
{
    public class CommentsInputDto : IMapTo<Comment>
    {
        [Required]
        public string Text { get; set; }

        public int? ReplyToId { get; set; }
    }
}
