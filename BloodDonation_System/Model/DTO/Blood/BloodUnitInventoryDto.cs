// File: BloodDonation_System.Model.DTO.Blood.BloodUnitInventoryDto.cs

using System;

namespace BloodDonation_System.Model.DTO.Blood
{
    public class BloodUnitInventoryDto
    {
        public string UnitId { get; set; } = null!;
        public string? DonationId { get; set; }

        public int BloodTypeId { get; set; }
        public string? BloodTypeName { get; set; } // Tên nhóm máu (ví dụ: "A+", "O-")

        public int ComponentId { get; set; }
        public string? ComponentName { get; set; } // Tên thành phần máu (ví dụ: "Whole Blood", "Plasma")

        public int VolumeMl { get; set; }
        public DateOnly CollectionDate { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public string? StorageLocation { get; set; }
        public string? TestResults { get; set; }
        public string? Status { get; set; } // Trạng thái của đơn vị máu (Available, Used, Expired, Discarded)
        public string? DiscardReason { get; set; }
    }
}