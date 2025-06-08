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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var profile = await _userProfileService.GetProfileByIdAsync(id);
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

            var (lat, lon) = await _geocodingService.GetCoordinatesFromAddressAsync(dto.Address);
            dto.Latitude = lat;
            dto.Longitude = lon;

            try
            {
                var created = await _userProfileService.CreateProfileAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ProfileId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserProfileDto dto)
        {
            var validationError = ValidateOptionalFields(dto);
            if (validationError != null)
                return BadRequest(validationError);

            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                var (lat, lon) = await _geocodingService.GetCoordinatesFromAddressAsync(dto.Address);
                dto.Latitude = lat;
                dto.Longitude = lon;
            }

            try
            {
                var updated = await _userProfileService.UpdateProfileAsync(id, dto);
                if (updated == null)
                    return NotFound("Profile not found for update.");

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _userProfileService.DeleteProfileAsync(id);
            return deleted ? NoContent() : NotFound("Profile not found.");
        }

        // ✅ Validation cho các trường không bắt buộc (nếu có nhập thì phải hợp lệ)
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
                dto.Gender != "Male" && dto.Gender != "Female" && dto.Gender != "Other")
                return "Gender must be 'Male', 'Female', or 'Other'.";

            if (!string.IsNullOrWhiteSpace(dto.Cccd) &&
                !Regex.IsMatch(dto.Cccd, @"^\d{12}$"))
                return "Citizen ID must contain exactly 12 digits.";

            return null;
        }
    }
}