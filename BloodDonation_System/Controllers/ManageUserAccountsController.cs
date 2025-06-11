using BloodDonation_System.Model.DTO.User;
using BloodDonation_System.Models.DTOs.User;
using DrugUsePreventionAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin được phép quản lý tài khoản
    public class ManageUserAccountsController : ControllerBase
    {
        private readonly IUserService _userService;

        public ManageUserAccountsController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/ManageUserAccounts
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/ManageUserAccounts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // POST: api/ManageUserAccounts
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }

        // PUT: api/ManageUserAccounts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            if (updatedUser == null)
                return NotFound("User not found");

            return Ok(updatedUser);
        }

        // PUT: api/ManageUserAccounts/{id}/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateUserRoleDto roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var callerRole = User.IsInRole("Admin") ? "Admin" : "Other"; // Có thể mở rộng xác thực phân quyền chi tiết
            var updatedUser = await _userService.UpdateUserRoleAsync(id, roleDto.RoleName, callerRole);

            if (updatedUser == null)
                return NotFound("User or role not found");

            return Ok(updatedUser);
        }

        // DELETE: api/ManageUserAccounts/{id}
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
