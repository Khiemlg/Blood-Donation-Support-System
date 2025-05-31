using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.MyModels;

[Table("DonationHistory")]
public partial class DonationHistory
{
    [Key]
    [Column("donation_id")]
    public int DonationId { get; set; }

    [Column("donor_user_id")]
    public int DonorUserId { get; set; }

    [Column("donation_date")]
    public DateTime DonationDate { get; set; }

    [Column("blood_type_id")]
    public int BloodTypeId { get; set; }

    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("quantity_ml")]
    public int? QuantityMl { get; set; }

    [Column("eligibility_status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? EligibilityStatus { get; set; }

    [Column("reason_ineligible")]
    public string? ReasonIneligible { get; set; }

    [Column("testing_results")]
    public string? TestingResults { get; set; }

    [Column("staff_user_id")]
    public int? StaffUserId { get; set; }

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("DonationHistories")]
    public virtual BloodType BloodType { get; set; } = null!;

    [InverseProperty("Donation")]
    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    [ForeignKey("ComponentId")]
    [InverseProperty("DonationHistories")]
    public virtual BloodComponent Component { get; set; } = null!;

    [ForeignKey("DonorUserId")]
    [InverseProperty("DonationHistoryDonorUsers")]
    public virtual User DonorUser { get; set; } = null!;

    [ForeignKey("StaffUserId")]
    [InverseProperty("DonationHistoryStaffUsers")]
    public virtual User? StaffUser { get; set; }
}
