using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("BloodTypeId", Name = "IX_MemberProfiles_BloodTypeID")]
[Index("IsEligibleToDonate", Name = "IX_MemberProfiles_IsEligibleToDonate")]
[Index("LastDonationDate", Name = "IX_MemberProfiles_LastDonationDate")]
[Index("UserId", Name = "UQ__MemberPr__1788CCADEAA81DA6", IsUnique = true)]
public partial class MemberProfile
{
    [Key]
    [Column("MemberProfileID")]
    public int MemberProfileId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("BloodTypeID")]
    public int? BloodTypeId { get; set; }

    [Column("RHFactor")]
    [StringLength(5)]
    public string? Rhfactor { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Weight { get; set; }

    public string? HealthStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastDonationDate { get; set; }

    public bool IsEligibleToDonate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedHealthStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("MemberProfiles")]
    public virtual BloodType? BloodType { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("MemberProfile")]
    public virtual User User { get; set; } = null!;
}
