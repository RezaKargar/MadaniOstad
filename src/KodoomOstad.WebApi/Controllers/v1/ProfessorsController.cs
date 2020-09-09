using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.WebApi.Models.Answers;
using KodoomOstad.WebApi.Models.Comments;
using KodoomOstad.WebApi.Models.Courses;
using KodoomOstad.WebApi.Models.Professors;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IRepository<Faculty> _facultyRepository;

        public ProfessorsController(IMapper mapper, IRepository<Professor> professorRepository, IRepository<Faculty> facultyRepository)
        {
            _mapper = mapper;
            _professorRepository = professorRepository;
            _facultyRepository = facultyRepository;
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

            var faculty = await _facultyRepository.GetByIdAsync(cancellationToken, dto.FacultyId);

            if (faculty == null)
                return NotFound("Faculty not found.");

            await _professorRepository.AddAsync(professor, cancellationToken);

            var createdProfessor = _mapper.Map<ProfessorOutputDto>(professor);

            return Created($"/api/v1/Professors/{createdProfessor.Id}", createdProfessor);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, ProfessorInputDto dto, CancellationToken cancellationToken)
        {
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, id);

            if (professor == null)
                return NotFound();

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
