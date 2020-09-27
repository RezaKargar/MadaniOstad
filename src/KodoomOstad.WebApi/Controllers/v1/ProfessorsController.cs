using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.Services.Services;
using KodoomOstad.WebApi.Models.Answers;
using KodoomOstad.WebApi.Models.Comments;
using KodoomOstad.WebApi.Models.Courses;
using KodoomOstad.WebApi.Models.Professors;
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
    public class ProfessorsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Professor> _professorRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Faculty> _facultyRepository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;

        public ProfessorsController(IMapper mapper, IRepository<Professor> professorRepository, IRepository<Comment> commentRepository, IRepository<Faculty> facultyRepository, IJwtService jwtService, UserManager<User> userManager)
        {
            _mapper = mapper;
            _professorRepository = professorRepository;
            _commentRepository = commentRepository;
            _facultyRepository = facultyRepository;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var professors = await _professorRepository.TableNoTracking.ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<ProfessorOutputDto>>(professors);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound();

            var dto = _mapper.Map<ProfessorOutputDto>(professor);

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProfessorInputDto dto, CancellationToken cancellationToken)
        {
            var professor = _mapper.Map<Professor>(dto);

            var isSlugOrNameDuplicated = await _professorRepository.TableNoTracking.AnyAsync(p => p.Name == dto.Name || p.Slug == dto.Slug, cancellationToken);

            if (isSlugOrNameDuplicated)
                return BadRequest("Professor with same 'Name' or 'Slug' already exists.");


            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, dto.FacultyId);

            if (faculty == null)
                return NotFound("Faculty not found.");

            await _professorRepository.AddAsync(professor, cancellationToken);

            var createdProfessor = _mapper.Map<ProfessorOutputDto>(professor);

            return Created($"api/v1/Professors/{createdProfessor.Id}", createdProfessor);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, ProfessorInputDto dto, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound();

            if (professor.Name != dto.Name || professor.Slug != dto.Slug)
            {
                var isSlugDuplicated = await _professorRepository
                    .TableNoTracking
                    .AnyAsync(p => p.Name == dto.Name || p.Slug == dto.Slug, cancellationToken);

                if (isSlugDuplicated)
                    return BadRequest("Repository with same 'Name' or 'Slug' already exists.");
            }

            if (dto.FacultyId != professor.FacultyId)
            {
                var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, dto.FacultyId);

                if (faculty == null)
                    return NotFound("Faculty not found.");
            }

            _mapper.Map(dto, professor);

            await _professorRepository.UpdateAsync(professor, cancellationToken);

            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound();

            await _professorRepository.DeleteAsync(professor, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}/Answers")]
        public async Task<IActionResult> Answers(int id, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Answers, cancellationToken);

            var answers = _mapper.Map<List<AnswersOutputDto>>(professor.Answers);

            return Ok(answers);
        }

        [HttpGet("{professorId}/Answers/{answerId}")]
        public async Task<IActionResult> Answers(int professorId, int answerId, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, professorId);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Answers, cancellationToken);

            var answer = professor.Answers.SingleOrDefault(a => a.Id == answerId);

            if (answer == null)
                return NotFound("Answer of professor not found.");

            var dto = _mapper.Map<AnswersOutputDto>(answer);

            return Ok(dto);
        }

        [Authorize]
        [HttpPost("{id:int}/Comments")]
        public async Task<IActionResult> AddComment(int id, CommentsInputDto dto, CancellationToken cancellationToken)
        {
            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var user = await _userManager.FindByIdAsync(userWhoSentRequest.Id.ToString());

            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

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
            comment.ProfessorId = professor.Id;

            await _commentRepository.AddAsync(comment, cancellationToken);

            var createdComment = _mapper.Map<CommentsOutputDto>(comment);

            return Created($"api/v1/Professors/{professor.Id}/Comments/{createdComment.Id}", createdComment);
        }

        [Authorize]
        [HttpPut("{professorId:int}/Comments/{commentId:int}")]
        public async Task<IActionResult> UpdateComment(int professorId, int commentId, CommentsUpdateInputDto dto, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, professorId);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Comments, cancellationToken);

            var comment = professor.Comments.SingleOrDefault(a => a.Id == commentId);

            if (comment == null)
                return NotFound("Comment of professor not found.");

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
        [HttpDelete("{professorId:int}/Comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int professorId, int commentId, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, professorId);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Comments, cancellationToken);

            var comment = professor.Comments.SingleOrDefault(a => a.Id == commentId);

            if (comment == null)
                return NotFound("Comment of professor not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isUserHimSelfRequesting = userWhoSentRequest.Id == comment.UserId;

            if (!isUserHimSelfRequesting)
                return Forbid();

            await _commentRepository.DeleteAsync(comment, cancellationToken);

            return NoContent();
        }

        [HttpGet("{id}/Comments")]
        public async Task<IActionResult> Comments(int id, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Comments, cancellationToken);

            var comments = _mapper.Map<List<CommentsOutputDto>>(professor.Comments);

            return Ok(comments);
        }

        [HttpGet("{professorId}/Comments/{commentId}")]
        public async Task<IActionResult> Comments(int professorId, int commentId, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, professorId);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Comments, cancellationToken);

            var comment = professor.Comments.SingleOrDefault(a => a.Id == commentId);

            if (comment == null)
                return NotFound("Comment of professor not found.");

            var dto = _mapper.Map<CommentsOutputDto>(comment);

            return Ok(dto);
        }

        [HttpGet("{id}/Courses")]
        public async Task<IActionResult> Courses(int id, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Courses, cancellationToken);

            var courses = _mapper.Map<List<CoursesOutputDto>>(professor.Courses);

            return Ok(courses);
        }

        [HttpGet("{professorId}/Courses/{courseId}")]
        public async Task<IActionResult> Courses(int professorId, int courseId, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, professorId);

            if (professor == null)
                return NotFound("Professor not found.");

            await _professorRepository.LoadCollectionAsync(professor, p => p.Courses, cancellationToken);

            var course = professor.Courses.SingleOrDefault(a => a.Id == courseId);

            if (course == null)
                return NotFound("Course of professor not found.");

            var dto = _mapper.Map<CoursesOutputDto>(course);

            return Ok(dto);
        }
    }
}
