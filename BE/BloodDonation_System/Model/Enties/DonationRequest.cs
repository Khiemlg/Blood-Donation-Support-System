using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Model.Enties;

public partial class DonationRequest
{
    [Key]
    [Column("request_id")]
    [StringLength(36)]
    public string RequestId { get; set; } = null!;

    [Column("donor_user_id")]
    [StringLength(36)]
    public string DonorUserId { get; set; } = null!;

    [Column("blood_type_id")]
    public int BloodTypeId { get; set; }

    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("preferred_date")]
    public DateOnly? PreferredDate { get; set; }

    [Column("preferred_time_slot")]
    [StringLength(50)]
    public string? PreferredTimeSlot { get; set; }

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("request_date")]
    public DateTime? RequestDate { get; set; }

    [Column("staff_notes")]
    public string? StaffNotes { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("DonationRequests")]
    public virtual BloodType BloodType { get; set; } = null!;

    [ForeignKey("ComponentId")]
    [InverseProperty("DonationRequests")]
    public virtual BloodComponent Component { get; set; } = null!;

    [ForeignKey("DonorUserId")]
    [InverseProperty("DonationRequests")]
    public virtual User DonorUser { get; set; } = null!;
    [InverseProperty("DonationRequest")] // Đây là Navigation Property trong DonationHistory trỏ về DonationRequest (tên là "DonationRequest")
    public virtual ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();
}
