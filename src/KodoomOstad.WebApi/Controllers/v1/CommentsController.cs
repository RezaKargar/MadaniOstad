using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.Services.Services;
using KodoomOstad.WebApi.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KodoomOstad.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    [Authorize(Roles = "Admin")]
    public class CommentsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Professor> _professorRepository;
        private readonly IJwtService _jwtService;

        public CommentsController(IMapper mapper, IRepository<Comment> commentRepository, UserManager<User> userManager, IRepository<Professor> professorRepository, IJwtService jwtService)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _userManager = userManager;
            _professorRepository = professorRepository;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.TableNoTracking.ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<CommentsOutputDto>>(comments);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, id);

            if (comment == null)
                return NotFound();

            var dto = _mapper.Map<CommentsOutputDto>(comment);

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, id);

            if (comment == null)
                return NotFound();

            await _commentRepository.DeleteAsync(comment, cancellationToken);

            return NoContent();
        }

        [HttpGet("{id}/Replies")]
        public async Task<IActionResult> Replies(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, id);

            if (comment == null)
                return NotFound();

            await _commentRepository.LoadCollectionAsync(comment, c => c.Replies, cancellationToken);

            var replies = _mapper.Map<List<CommentsOutputDto>>(comment.Replies);

            return Ok(replies);
        }

        [HttpGet("{commentId}/Replies/{replyId}")]
        public async Task<IActionResult> Replies(int commentId, int replyId, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, commentId);

            if (comment == null)
                return NotFound("Comment not found.");

            await _commentRepository.LoadCollectionAsync(comment, c => c.Replies, cancellationToken);

            var reply = comment.Replies.SingleOrDefault(r => r.Id == replyId);

            if (reply == null)
                return NotFound("Reply of comment not found.");

            var dto = _mapper.Map<CommentsOutputDto>(reply);

            return Ok(dto);
        }
    }
}
