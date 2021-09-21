using AutoMapper;
using MadaniOstad.Entities.Models;
using MadaniOstad.WebApi.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MadaniOstad.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpGet("{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string name, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/Users")]
        public async Task<IActionResult> Users(int id, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                return NotFound("Role not found.");

            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            var dtos = _mapper.Map<List<UsersOutputDto>>(users);

            return Ok(dtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{name}/Users")]
        public async Task<IActionResult> Users(string name, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
                return NotFound("Role not found.");

            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            var dtos = _mapper.Map<List<UsersOutputDto>>(users);

            return Ok(dtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{roleId:int}/Users/{userId:int}")]
        public async Task<IActionResult> Users(int roleId, int userId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
                return NotFound("Role not found.");

            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            var user = users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            var dto = _mapper.Map<UsersOutputDto>(user);

            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{roleName}/Users/{userId:int}")]
        public async Task<IActionResult> Users(string roleName, int userId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                return NotFound("Role not found.");

            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            var user = users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            var dto = _mapper.Map<UsersOutputDto>(user);

            return Ok(dto);
        }
    }
}
