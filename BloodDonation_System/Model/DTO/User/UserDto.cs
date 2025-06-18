namespace BloodDonation_System.Model.DTO.User
{
    public class UserDto
    {
        // Primary Identifier
        public string UserId { get; set; } = null!; // Kept as UserId for camelCase JSON output

        public string Username { get; set; } = null!; // Kept as Username for camelCase JSON output
        public string PasswordHash { get; set; } = null!; // PasswordHash - likely for sending, not receiving sensitive info directly in DTO
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!; // This property was not in the error list, but assume it should be there.
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? IsActive { get; set; }

        // Consider if these should be settable or only readable, depends on context
        // If they are internal set, ensure they are set during object creation or mapping.
        public DateTime UpdatedAt { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}