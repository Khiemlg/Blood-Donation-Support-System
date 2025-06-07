using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Model.Enties;

[Index("PhoneNumber", Name = "UQ__UserProf__A1936A6BF483ACF9", IsUnique = true)]
[Index("Cccd", Name = "UQ__UserProf__A955A0AA4BF11E90", IsUnique = true)]
[Index("UserId", Name = "UQ__UserProf__B9BE370E3E5D67D8", IsUnique = true)]
public partial class UserProfile
{
    [Key]
    [Column("profile_id")]
    [StringLength(36)]
    public string ProfileId { get; set; } = null!;

    [Column("user_id")]
    [StringLength(36)]
    public string UserId { get; set; } = null!;

    [Column("full_name")]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("gender")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Gender { get; set; }

    [Column("address")]
    [StringLength(255)]
    public string? Address { get; set; }

    [Column("latitude", TypeName = "decimal(10, 8)")]
    public decimal? Latitude { get; set; }

    [Column("longitude", TypeName = "decimal(11, 8)")]
    public decimal? Longitude { get; set; }

    [Column("blood_type_id")]
    public int? BloodTypeId { get; set; }

    [Column("rh_factor")]
    [StringLength(10)]
    [Unicode(false)]
    public string? RhFactor { get; set; }

    [Column("medical_history")]
    public string? MedicalHistory { get; set; }

    [Column("last_blood_donation_date")]
    public DateOnly? LastBloodDonationDate { get; set; }

    [Column("CCCD")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Cccd { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("UserProfiles")]
    public virtual BloodType? BloodType { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserProfile")]
    public virtual User User { get; set; } = null!;
}
