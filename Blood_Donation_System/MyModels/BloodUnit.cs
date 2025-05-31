using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.MyModels;

public partial class BloodUnit
{
    [Key]
    [Column("unit_id")]
    public int UnitId { get; set; }

    [Column("donation_id")]
    public int? DonationId { get; set; }

    [Column("blood_type_id")]
    public int BloodTypeId { get; set; }

    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("volume_ml")]
    public int VolumeMl { get; set; }

    [Column("collection_date")]
    public DateOnly CollectionDate { get; set; }

    [Column("expiration_date")]
    public DateOnly ExpirationDate { get; set; }

    [Column("storage_location")]
    [StringLength(100)]
    public string? StorageLocation { get; set; }

    [Column("test_results")]
    public string? TestResults { get; set; }

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("discard_reason")]
    public string? DiscardReason { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("BloodUnits")]
    public virtual BloodType BloodType { get; set; } = null!;

    [ForeignKey("ComponentId")]
    [InverseProperty("BloodUnits")]
    public virtual BloodComponent Component { get; set; } = null!;

    [ForeignKey("DonationId")]
    [InverseProperty("BloodUnits")]
    public virtual DonationHistory? Donation { get; set; }
}
