using AutoMapper;
using KodoomOstad.Entities.Models;
using KodoomOstad.Services.Services;
using KodoomOstad.WebApi.Models.Answers;
using KodoomOstad.WebApi.Models.AuthenticationModel;
using KodoomOstad.WebApi.Models.Comments;
using KodoomOstad.WebApi.Models.Courses;
using KodoomOstad.WebApi.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using KodoomOstad.Common.Utilities;

namespace KodoomOstad.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class UsersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;

        public UsersController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Authenticate(AuthenticationModel model, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized(new
                {
                    Errors = new[] { "Email or password is incorrect" }
                });

            var isValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

            if (!isValid.Succeeded)
            {
                if (!isValid.IsLockedOut)
                    return Unauthorized(new
                    {
                        Errors = new[] { "User attempting to sign-in is lock out for a few minutes" }
                    });
                return Unauthorized(new
                {
                    Errors = new[] { "Email or password is incorrect" }
                });
            }

            var token = await _jwtService.GenerateAsync(user);

            return Ok(token);

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AuthenticateForSwagger([FromForm] AuthenticationModel model, CancellationToken cancellationToken)
        {
            return await Authenticate(model, cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsersCreateInputDto dto, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(dto);

            if (string.IsNullOrWhiteSpace(user.UserName))
                user.UserName = user.Email;

            var isCreated = await _userManager.CreateAsync(user, password: dto.Password);

            if (!isCreated.Succeeded)
                return BadRequest(isCreated.Errors);

            var createdUser = _mapper.Map<UsersOutputDto>(user);

            return Created("api/v1/Users/" + createdUser.Id, createdUser);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var userDtos = _mapper.Map<List<UsersOutputDto>>(_userManager.Users);

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UsersOutputDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UsersUpdateInputDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfUpdating = userWhoSentRequest.Id == user.Id;
            
            if (!isAdmin && !isUserHimSelfUpdating)
                return Forbid();

            _mapper.Map(dto, user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var token = _jwtService.ReadToken(Request.Headers["Authorization"]);

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                return BadRequest(deleteResult.Errors);

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Comments(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Comments).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var comments = _mapper.Map<List<CommentsOutputDto>>(user.Comments);

            return Ok(comments);
        }

        [HttpGet("{userId}/[action]/{commentId}")]
        public async Task<IActionResult> Comments(int userId, int commentId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Comments).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var comment = user.Comments.SingleOrDefault(c => c.Id == commentId);

            if (comment == null)
                return NotFound("Comment not found.");


            var dto = _mapper.Map<CommentsOutputDto>(comment);

            return Ok(dto);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Courses(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var courses = _mapper.Map<List<CoursesOutputDto>>(user.Courses);

            return Ok(courses);
        }

        [HttpGet("{userId}/[action]/{courseId}")]
        public async Task<IActionResult> Courses(int userId, int courseId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var course = user.Courses.SingleOrDefault(c => c.Id == courseId);

            if (course == null)
                return NotFound("Course not found.");


            var dto = _mapper.Map<CoursesOutputDto>(course);

            return Ok(dto);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Answers(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Answers).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var answers = _mapper.Map<List<AnswersOutputDto>>(user.Answers);

            return Ok(answers);
        }

        [HttpGet("{userId}/[action]/{answerId}")]
        public async Task<IActionResult> Answers(int userId, int answerId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Answers).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var answer = user.Answers.SingleOrDefault(c => c.Id == answerId);

            if (answer == null)
                return NotFound("Answer not found.");


            var dto = _mapper.Map<AnswersOutputDto>(answer);

            return Ok(dto);
        }

    }
}
