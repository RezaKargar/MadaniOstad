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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CommentsInputDto dto, CancellationToken cancellationToken)
        {
            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var user = await _userManager.FindByIdAsync(userWhoSentRequest.Id.ToString());

            var professor = await _professorRepository.GetByIdAsync(cancellationToken, dto.ProfessorId);

            var notFoundMessages = new List<string>();

            if (user == null)
                notFoundMessages.Add("User not found.");

            if (professor == null)
                notFoundMessages.Add("Professor not found.");

            if (dto.ReplyToId != null)
            {
                var commentToReply = await _commentRepository.GetByIdAsync(cancellationToken, dto.ReplyToId);

                if (commentToReply == null)
                    notFoundMessages.Add("Comment which meant to be parent, not found.");
            }

            if (notFoundMessages.Any())
                return NotFound(notFoundMessages);


            var comment = _mapper.Map<Comment>(dto);
            comment.UserId = user.Id;

            await _commentRepository.AddAsync(comment, cancellationToken);

            var createdComment = _mapper.Map<CommentsOutputDto>(comment);

            return Created($"api/v1/Comments/{createdComment.Id}", createdComment);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CommentsUpdateInputDto dto, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, id);

            if (comment == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isUserHimSelfRequesting = userWhoSentRequest.Id == comment.UserId;

            if (!isUserHimSelfRequesting)
                return BadRequest("You can't update others' comment.");


            _mapper.Map(dto, comment);

            await _commentRepository.UpdateAsync(comment, cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, id);

            if (comment == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == comment.UserId;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

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
