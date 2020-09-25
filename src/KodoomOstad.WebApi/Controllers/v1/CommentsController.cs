using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.WebApi.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public CommentsController(IMapper mapper, IRepository<Comment> commentRepository)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.TableNoTracking.Include(c => c.Replies).ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<CommentsOutputDto>>(comments);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(cancellationToken, id);

            if (comment == null)
                return NotFound();

            await _commentRepository.LoadCollectionAsync(comment, c => c.Replies, cancellationToken);

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
    }
}
