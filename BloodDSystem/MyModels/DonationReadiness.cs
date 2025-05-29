using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Table("DonationReadiness")]
[Index("BloodTypeId", Name = "IX_DonationReadiness_BloodTypeID")]
[Index("ReadyDate", "IsActive", Name = "IX_DonationReadiness_ReadyDate_IsActive")]
[Index("UserId", Name = "IX_DonationReadiness_UserID")]
public partial class DonationReadiness
{
    [Key]
    [Column("DonationReadinessID")]
    public int DonationReadinessId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("BloodTypeID")]
    public int BloodTypeId { get; set; }

    [Column("ComponentID")]
    public int? ComponentId { get; set; }

    public DateOnly ReadyDate { get; set; }

    [StringLength(50)]
    public string? PreferredTimeSlot { get; set; }

    public bool IsActive { get; set; }

    [Column("ConfirmedByUserID")]
    public int? ConfirmedByUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ConfirmedAt { get; set; }

    public string? Notes { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("DonationReadinesses")]
    public virtual BloodType BloodType { get; set; } = null!;

    [ForeignKey("ComponentId")]
    [InverseProperty("DonationReadinesses")]
    public virtual BloodComponent? Component { get; set; }

    [ForeignKey("ConfirmedByUserId")]
    [InverseProperty("DonationReadinessConfirmedByUsers")]
    public virtual User? ConfirmedByUser { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("DonationReadinessUsers")]
    public virtual User User { get; set; } = null!;
}
