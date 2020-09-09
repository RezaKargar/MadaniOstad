using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.Services.Services;
using KodoomOstad.WebApi.Models.Courses;
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
    public class CoursesController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Course> _courseRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Professor> _professorRepository;
        private readonly IJwtService _jwtService;

        public CoursesController(IMapper mapper, IRepository<Course> courseRepository, UserManager<User> userManager, IRepository<Professor> professorRepository, IJwtService jwtService)
        {
            _mapper = mapper;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _professorRepository = professorRepository;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.TableNoTracking.ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<CoursesOutputDto>>(courses);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(cancellationToken, id);

            if (course == null)
                return NotFound();

            var dto = _mapper.Map<CoursesOutputDto>(course);

            return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CourseInputDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            var professor = await _professorRepository.GetByIdAsync(cancellationToken, dto.ProfessorId);

            var notFoundMessages = new List<string>();

            if (user == null)
            {
                notFoundMessages.Add("User not found.");
            }

            if (professor == null)
            {
                notFoundMessages.Add("Professor not found.");
            }

            if (notFoundMessages.Any())
                return NotFound(notFoundMessages);

            var course = _mapper.Map<Course>(dto);

            await _courseRepository.AddAsync(course, cancellationToken);

            var createdCourse = _mapper.Map<CoursesOutputDto>(course);

            return Created($"/api/v1/Courses/{createdCourse.Id}", createdCourse);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, CourseInputDto dto, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(cancellationToken, id);

            if (course == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserCourseCreator = userWhoSentRequest.Id == course.UserId;

            if (!isAdmin && !isUserCourseCreator)
                return Forbid();


            var notFoundMessages = new List<string>();

            if (dto.UserId != course.UserId)
            {
                var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

                if (user == null)
                {
                    notFoundMessages.Add("User not found.");
                }
            }

            if (dto.ProfessorId != course.ProfessorId)
            {
                var professor = await _professorRepository.GetByIdAsync(cancellationToken, dto.ProfessorId);

                if (professor == null)
                {
                    notFoundMessages.Add("Professor not found.");
                }
            }

            if (notFoundMessages.Any())
                return NotFound(notFoundMessages);

            _mapper.Map(dto, course);

            await _courseRepository.UpdateAsync(course, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(cancellationToken, id);

            if (course == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserCourseCreator = userWhoSentRequest.Id == course.UserId;

            if (!isAdmin && !isUserCourseCreator)
                return Forbid();

            await _courseRepository.DeleteAsync(course, cancellationToken);

            return NoContent();
        }
    }
}
