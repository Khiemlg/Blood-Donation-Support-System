namespace BloodDonation_System.Model.DTO.Donation
{
    public class DonationRequestDto
    {
        public string RequestId { get; set; }
        public string DonorUserId { get; set; }
        public int BloodTypeId { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; } 
        public DateOnly? PreferredDate { get; set; }
        public string? PreferredTimeSlot { get; set; }
        public string? Status { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? StaffNotes { get; set; }
        public string? DonorUserName { get; set; }
        public string? BloodTypeName { get; set; }
    }
}
