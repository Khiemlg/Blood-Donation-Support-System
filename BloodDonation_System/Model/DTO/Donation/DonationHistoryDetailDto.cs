namespace BloodDonation_System.Model.DTO.Donation
{
    public class DonationHistoryDetailDto
    {
        public string DonationId { get; set; } = null!; 
        public string DonorUserId { get; set; } = null!; 
        public string? DonorUserName { get; set; }
        public DateTime DonationDate { get; set; }
        public int BloodTypeId { get; set; } 
        public string? BloodTypeName { get; set; } 
        public int ComponentId { get; set; } 
        public string? ComponentName { get; set; } 
        public int QuantityMl { get; set; }
        public string EligibilityStatus { get; set; } = null!;
        public string? ReasonIneligible { get; set; }
        public string? TestingResults { get; set; }
        public string? StaffUserId { get; set; }
        public string? Status { get; set; }
        public string? EmergencyId { get; set; }
        public string? Descriptions { get; set; }
        public string? DonationRequestId { get; set; }
    }
}