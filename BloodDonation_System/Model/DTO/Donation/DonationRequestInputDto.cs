using System;
using System.ComponentModel.DataAnnotations;

namespace BloodDonation_System.Model.DTO.Donation
{
    public class DonationRequestInputDto
    {
      
        [StringLength(36, ErrorMessage = "DonorUserId must be 36 characters.")]
        public string? DonorUserId { get; set; }

        [Required(ErrorMessage = "BloodTypeId is required.")]
        public int BloodTypeId { get; set; }

        [Required(ErrorMessage = "ComponentId is required.")]
        public int ComponentId { get; set; }

        public DateOnly? PreferredDate { get; set; }

        [StringLength(50, ErrorMessage = "PreferredTimeSlot cannot exceed 50 characters.")]
        public string? PreferredTimeSlot { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string? Status { get; set; } 

        public string? StaffNotes { get; set; }
    }
}
