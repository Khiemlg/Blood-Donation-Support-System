using BloodDonation_System.Model.DTO.UserProfile;
using BloodDonation_System.Service.Interface;
using BloodDonation_System.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BloodDonation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly GeocodingService _geocodingService;

        public UserProfileController(
            IUserProfileService userProfileService,
            GeocodingService geocodingService)
        {
            _userProfileService = userProfileService;
            _geocodingService = geocodingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _userProfileService.GetAllProfilesAsync();
            return Ok(profiles);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var profile = await _userProfileService.GetProfileByUserIdAsync(userId);
            if (profile == null)
                return NotFound("Profile not found.");
            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest("UserId and FullName are required.");

            var validationError = ValidateOptionalFields(dto);
            if (validationError != null)
                return BadRequest(validationError);

            var trimmedAddress = dto.Address?.Trim();

            if (!string.IsNullOrWhiteSpace(trimmedAddress))
            {
                var (lat, lon) = await _geocodingService.GetCoordinatesFromAddressAsync(trimmedAddress);
                if (lat == 0 && lon == 0)
                    return BadRequest("Địa chỉ không hợp lệ, vui lòng nhập chi tiết hơn (ít nhất quận/huyện và thành phố).");

                dto.Address = trimmedAddress;
                dto.Latitude = lat;
                dto.Longitude = lon;
            }

            try
            {
                var created = await _userProfileService.CreateProfileAsync(dto);
                return CreatedAtAction(nameof(GetByUserId), new { userId = created.UserId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("by-user/{userId}")]
        public async Task<IActionResult> UpdateByUserId(string userId, [FromBody] UpdateUserProfileDto dto)
        {
            var validationError = ValidateOptionalFields(dto);
            if (validationError != null)
                return BadRequest(validationError);

            var trimmedAddress = dto.Address?.Trim();

            if (!string.IsNullOrWhiteSpace(trimmedAddress))
            {
                var (lat, lon) = await _geocodingService.GetCoordinatesFromAddressAsync(trimmedAddress);
                if (lat == 0 && lon == 0)
                    return BadRequest("Địa chỉ không hợp lệ, vui lòng nhập chi tiết hơn (ít nhất quận/huyện và thành phố).");

                dto.Address = trimmedAddress;
                dto.Latitude = lat;
                dto.Longitude = lon;
            }

            try
            {
                var updated = await _userProfileService.UpdateProfileByUserIdAsync(userId, dto);
                if (updated == null)
                    return NotFound("User profile not found for update.");

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("by-user/{userId}")]
        public async Task<IActionResult> DeleteByUserId(string userId)
        {
            var deleted = await _userProfileService.DeleteProfileByUserIdAsync(userId);
            return deleted ? NoContent() : NotFound("Profile not found.");
        }

        private string? ValidateOptionalFields(dynamic dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) &&
                !Regex.IsMatch(dto.PhoneNumber, @"^(0|\+84)[0-9]{9,10}$"))
                return "Invalid phone number.";

            if (dto.DateOfBirth is not null &&
                dto.DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
                return "Date of birth cannot be in the future.";

            if (dto.BloodTypeId is not null &&
                (dto.BloodTypeId < 1 || dto.BloodTypeId > 8))
                return "Invalid blood type ID.";

            if (!string.IsNullOrWhiteSpace(dto.Gender) &&
                dto.Gender != "Nam" && dto.Gender != "Nữ" && dto.Gender != "Khác")
                return "Gender must be 'Nam', 'Nữ', or 'Khác'.";

            if (!string.IsNullOrWhiteSpace(dto.Cccd) &&
                !Regex.IsMatch(dto.Cccd, @"^\d{12}$"))
                return "Citizen ID must contain exactly 12 digits.";

            return null;
        }
    }
}
