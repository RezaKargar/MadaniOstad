using AutoMapper;
using MadaniOstad.Entities.Models;
using MadaniOstad.Services.Services;
using MadaniOstad.WebApi.Models.Answers;
using MadaniOstad.WebApi.Models.AuthenticationModel;
using MadaniOstad.WebApi.Models.Comments;
using MadaniOstad.WebApi.Models.Common.ResponseExamples;
using MadaniOstad.WebApi.Models.Courses;
using MadaniOstad.WebApi.Models.Users;
using MadaniOstad.WebApi.Models.Users.RequestExamples.Authenticate;
using MadaniOstad.WebApi.Models.Users.RequestExamples.Create;
using MadaniOstad.WebApi.Models.Users.RequestExamples.Put;
using MadaniOstad.WebApi.Models.Users.ResponseExamples.Authenticate;
using MadaniOstad.WebApi.Models.Users.ResponseExamples.Create;
using MadaniOstad.WebApi.Models.Users.ResponseExamples.Delete;
using MadaniOstad.WebApi.Models.Users.ResponseExamples.Get;
using MadaniOstad.WebApi.Models.Users.ResponseExamples.GetList;
using MadaniOstad.WebApi.Models.Users.ResponseExamples.Put;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MadaniOstad.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class UsersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UsersController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService, RoleManager<IdentityRole<int>> roleManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
        }

        [HttpPost("[action]")]
        [SwaggerRequestExample(typeof(AuthenticationModel), typeof(UserAuthenticateRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserAuthenticateOkResponseExample))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UserAuthenticateUnauthorizedResponseExample))]
        public async Task<IActionResult> Authenticate(AuthenticationModel model, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized("Email or password is incorrect");

            var isValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

            if (!isValid.Succeeded)
            {
                if (!isValid.IsLockedOut)
                    return Unauthorized("User attempting to sign-in is lock out for a few minutes");
                return Unauthorized("Email or password is incorrect");
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
        [SwaggerRequestExample(typeof(UsersCreateInputDto), typeof(UserCreateRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserCreateOkResponseExample))]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "api/v1/Users/1")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UserCreateBadRequestResponseExample))]
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
        [Authorize(Roles = "Admin")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserGetListOkResponseExample))]
        public IActionResult Get()
        {
            var userDtos = _mapper.Map<List<UsersOutputDto>>(_userManager.Users);

            return Ok(userDtos);
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserGetOkResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var userDto = _mapper.Map<UsersOutputDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerRequestExample(typeof(UsersUpdateInputDto), typeof(UserPutRequestExample))]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status204NoContent, typeof(NoContentResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UserPutBadRequestResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
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
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status204NoContent, typeof(NoContentResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UserDeleteBadRequestResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(object))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                return BadRequest(deleteResult.Errors);

            return NoContent();
        }

        [Authorize]
        [HttpPut("[action]/{id:int}")]
        public async Task<IActionResult> ChangePassword(int id, UsersChangePasswordInputDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimselfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimselfRequesting)
                return Forbid();

            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!passwordChangeResult.Succeeded)
                return BadRequest(passwordChangeResult.Errors);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]/{userId:int}/{roleId:int}")]
        public async Task<IActionResult> AddToRole(int userId, int roleId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return NotFound("Role not found.");

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addToRoleResult.Succeeded)
                return BadRequest(addToRoleResult.Errors);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]/{userId:int}/{roleName}")]
        public async Task<IActionResult> AddToRole(int userId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role not found.");

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addToRoleResult.Succeeded)
                return BadRequest(addToRoleResult.Errors);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]/{userId:int}/{roleName}")]
        public async Task<IActionResult> DeleteFromRole(int userId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role not found.");

            var deleteFromRoleResult = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if (!deleteFromRoleResult.Succeeded)
                return BadRequest(deleteFromRoleResult.Errors);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]/{userId:int}/{roleId:int}")]
        public async Task<IActionResult> DeleteFromRole(int userId, int roleId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return NotFound("Role not found.");

            var deleteFromRoleResult = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if (!deleteFromRoleResult.Succeeded)
                return BadRequest(deleteFromRoleResult.Errors);

            return NoContent();
        }

        [HttpGet("{id:int}/[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Roles(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var roleNames = await _userManager.GetRolesAsync(user);

            var roles = new List<IdentityRole<int>>();
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                roles.Add(role);
            }

            return Ok(roles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId:int}/[action]/{roleId:int}")]
        public async Task<IActionResult> Roles(int userId, int roleId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
                return NotFound("Role not found.");

            if (!roles.Any(r => r == role.Name))
                return NotFound("Role not found.");

            return Ok(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId:int}/[action]/{roleName}")]
        public async Task<IActionResult> Roles(int userId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any(r => r == roleName))
                return NotFound("Role not found.");

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                return NotFound("Role not found.");

            return Ok(role);
        }

        [Authorize]
        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Comments(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Comments).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var comments = _mapper.Map<List<CommentsOutputDto>>(user.Comments);

            return Ok(comments);
        }

        [Authorize]
        [HttpGet("{userId}/[action]/{commentId}")]
        public async Task<IActionResult> Comments(int userId, int commentId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Comments).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var comment = user.Comments.SingleOrDefault(c => c.Id == commentId);

            if (comment == null)
                return NotFound("Comment of user not found.");


            var dto = _mapper.Map<CommentsOutputDto>(comment);

            return Ok(dto);
        }

        [Authorize]
        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Courses(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var courses = _mapper.Map<List<CoursesOutputDto>>(user.Courses);

            return Ok(courses);
        }

        [Authorize]
        [HttpGet("{userId}/[action]/{courseId}")]
        public async Task<IActionResult> Courses(int userId, int courseId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var course = user.Courses.SingleOrDefault(c => c.Id == courseId);

            if (course == null)
                return NotFound("Course of user not found.");


            var dto = _mapper.Map<CoursesOutputDto>(course);

            return Ok(dto);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Answers(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Answers).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var answers = _mapper.Map<List<AnswersOutputDto>>(user.Answers);

            return Ok(answers);
        }

        [HttpGet("{userId}/[action]/{answerId}")]
        public async Task<IActionResult> Answers(int userId, int answerId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(u => u.Answers).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return NotFound("User not found.");

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == user.Id;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            var answer = user.Answers.SingleOrDefault(c => c.Id == answerId);

            if (answer == null)
                return NotFound("Answer of user not found.");


            var dto = _mapper.Map<AnswersOutputDto>(answer);

            return Ok(dto);
        }

    }
}
