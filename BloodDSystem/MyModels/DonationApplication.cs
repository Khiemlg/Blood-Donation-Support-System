using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("DesiredDonationDate", Name = "IX_DonationApplications_DesiredDonationDate")]
[Index("ApplicationStatus", Name = "IX_DonationApplications_Status")]
[Index("UserId", Name = "IX_DonationApplications_UserID")]
public partial class DonationApplication
{
    [Key]
    [Column("ApplicationID")]
    public int ApplicationId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string ApplicationType { get; set; } = null!;

    [Column("BloodTypeID")]
    public int? BloodTypeId { get; set; }

    [Column("RHFactor")]
    [StringLength(5)]
    public string? Rhfactor { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Weight { get; set; }

    public string? HealthStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastDonationDateSubmitted { get; set; }

    public DateOnly? DesiredDonationDate { get; set; }

    [StringLength(50)]
    public string? PreferredTimeSlot { get; set; }

    [StringLength(50)]
    public string ApplicationStatus { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? SubmittedAt { get; set; }

    [Column("ReviewedByUserID")]
    public int? ReviewedByUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewedAt { get; set; }

    public string? ReviewNotes { get; set; }

    [StringLength(50)]
    public string? ApplicationSource { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("DonationApplications")]
    public virtual BloodType? BloodType { get; set; }

    [ForeignKey("ReviewedByUserId")]
    [InverseProperty("DonationApplicationReviewedByUsers")]
    public virtual User? ReviewedByUser { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("DonationApplicationUsers")]
    public virtual User User { get; set; } = null!;
}
