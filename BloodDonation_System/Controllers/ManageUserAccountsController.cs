using BloodDonation_System.Model.DTO.User;
using BloodDonation_System.Models.DTOs.User;
using DrugUsePreventionAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize(Roles = "Admin, Staff")]

    public class ManageUserAccountsController : ControllerBase
    {
        private readonly IUserService _userService;

        public ManageUserAccountsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userService.CreateUserAsync(createUserDto);
            if (createdUser == null)
            {
                return BadRequest("Email đã tồn tại hoặc User name đã tồn tại");
            }
            else 
            
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            if (updatedUser == null)
                return NotFound("User not found");

            return Ok(updatedUser);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateUserRoleDto roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var callerRole = User.IsInRole("Admin") ? "Admin" : "Other"; 
            var updatedUser = await _userService.UpdateUserRoleAsync(id, roleDto.RoleName, callerRole);

            if (updatedUser == null)
                return NotFound("User or role not found");

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound("User not found");

            return Ok("User deleted successfully");
        }
    }
}
