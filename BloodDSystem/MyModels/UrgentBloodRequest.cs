using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("BloodTypeId", "ComponentId", Name = "IX_UrgentBloodRequests_BloodTypeID_ComponentID")]
[Index("CreatedByUserId", Name = "IX_UrgentBloodRequests_CreatedByUserID")]
[Index("Status", Name = "IX_UrgentBloodRequests_Status")]
public partial class UrgentBloodRequest
{
    [Key]
    [Column("RequestID")]
    public int RequestId { get; set; }

    [StringLength(255)]
    public string? PatientName { get; set; }

    [Column("BloodTypeID")]
    public int BloodTypeId { get; set; }

    [Column("ComponentID")]
    public int ComponentId { get; set; }

    [Column("VolumeML", TypeName = "decimal(7, 2)")]
    public decimal VolumeMl { get; set; }

    [StringLength(20)]
    public string UrgencyLevel { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? RequestedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NeededBy { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("CreatedByUserID")]
    public int CreatedByUserId { get; set; }

    [Column("ApprovedByUserID")]
    public int? ApprovedByUserId { get; set; }

    public string? Notes { get; set; }

    [StringLength(255)]
    public string? DepartmentName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ApprovedByUserId")]
    [InverseProperty("UrgentBloodRequestApprovedByUsers")]
    public virtual User? ApprovedByUser { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("UrgentBloodRequests")]
    public virtual BloodType BloodType { get; set; } = null!;

    [ForeignKey("ComponentId")]
    [InverseProperty("UrgentBloodRequests")]
    public virtual BloodComponent Component { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("UrgentBloodRequestCreatedByUsers")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("Request")]
    public virtual ICollection<DonationProcess> DonationProcesses { get; set; } = new List<DonationProcess>();
}
