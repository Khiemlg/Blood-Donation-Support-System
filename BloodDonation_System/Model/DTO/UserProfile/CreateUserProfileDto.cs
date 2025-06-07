namespace BloodDonation_System.Model.DTO.UserProfile
{
    public class CreateUserProfileDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? BloodTypeId { get; set; }
        public string? RhFactor { get; set; }
        public string? MedicalHistory { get; set; }
        public DateOnly? LastBloodDonationDate { get; set; }
        public string? Cccd { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
