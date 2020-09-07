using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;

namespace KodoomOstad.WebApi.Models.Comments
{
    public class CommentsOutputDto : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string CreatedAt { get; set; }

        public string LastModifiedAt { get; set; }

        public int? ReplyToId { get; set; }

        public int ProfessorId { get; set; }

        public int UserId { get; set; }
    }
}
