namespace BloodDonation_System.Model.DTO.Donation
{
    public class DonationHistoryUpdateDto
    {
        public string DonorUserId { get; set; } = null!;
        public DateTime DonationDate { get; set; }
        public int BloodTypeId { get; set; }
        public int ComponentId { get; set; }
        public int? QuantityMl { get; set; }
        public string? EligibilityStatus { get; set; }
        public string? ReasonIneligible { get; set; }
        public string? TestingResults { get; set; }
        public string? StaffUserId { get; set; }
        public string? Status { get; set; }
        public string? EmergencyId { get; set; }
        public string? Descriptions { get; set; }
        public string? DonationRequestId { get; internal set; }
    }
}
