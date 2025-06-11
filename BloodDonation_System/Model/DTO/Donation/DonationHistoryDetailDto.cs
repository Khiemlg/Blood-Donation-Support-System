// File: BloodDonation_System.Model.DTO.Donation.DonationHistoryDetailDto.cs
namespace BloodDonation_System.Model.DTO.Donation
{
    public class DonationHistoryDetailDto
    {
        public string DonationId { get; set; } = null!; // Should not be nullable if it's the key
        public string DonorUserId { get; set; } = null!; // Should not be nullable if it's a required foreign key
        public string? DonorUserName { get; set; } // Thêm trường tên người hiến
        public DateTime DonationDate { get; set; }
        public int BloodTypeId { get; set; } // int as per your entity
        public string? BloodTypeName { get; set; } // Thêm trường tên nhóm máu
        public int ComponentId { get; set; } // int as per your entity
        public string? ComponentName { get; set; } // Thêm trường tên thành phần máu
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