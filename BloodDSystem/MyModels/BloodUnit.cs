using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("BloodTypeId", "ComponentId", Name = "IX_BloodUnits_BloodTypeID_ComponentID")]
[Index("DonorUserId", Name = "IX_BloodUnits_DonorUserID")]
[Index("ExpiryDate", Name = "IX_BloodUnits_ExpiryDate")]
[Index("Status", Name = "IX_BloodUnits_Status")]
[Index("UnitCode", Name = "UQ__BloodUni__0665E6D9736B1B8B", IsUnique = true)]
public partial class BloodUnit
{
    [Key]
    [Column("UnitID")]
    public int UnitId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UnitCode { get; set; } = null!;

    [Column("DonorUserID")]
    public int DonorUserId { get; set; }

    [Column("BloodTypeID")]
    public int BloodTypeId { get; set; }

    [Column("ComponentID")]
    public int ComponentId { get; set; }

    [Column("VolumeML", TypeName = "decimal(7, 2)")]
    public decimal VolumeMl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CollectedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }

    [StringLength(100)]
    public string? StorageLocation { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("CollectedByUserID")]
    public int CollectedByUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UsageDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("BloodUnits")]
    public virtual BloodType BloodType { get; set; } = null!;

    [ForeignKey("CollectedByUserId")]
    [InverseProperty("BloodUnitCollectedByUsers")]
    public virtual User CollectedByUser { get; set; } = null!;

    [ForeignKey("ComponentId")]
    [InverseProperty("BloodUnits")]
    public virtual BloodComponent Component { get; set; } = null!;

    [InverseProperty("UsedBloodUnit")]
    public virtual ICollection<DonationProcess> DonationProcesses { get; set; } = new List<DonationProcess>();

    [ForeignKey("DonorUserId")]
    [InverseProperty("BloodUnitDonorUsers")]
    public virtual User DonorUser { get; set; } = null!;
}
