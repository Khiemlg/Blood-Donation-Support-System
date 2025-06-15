using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Model.Enties;

[Index("TypeName", Name = "UQ__BloodTyp__543C4FD9D5BB432A", IsUnique = true)]
public partial class BloodType
{
    [Key]
    [Column("blood_type_id")]
    public int BloodTypeId { get; set; }

    [Column("type_name")]
    [StringLength(10)]
    [Unicode(false)]
    public string TypeName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [InverseProperty("BloodType")]
    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    [InverseProperty("BloodType")]
    public virtual ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();

    [InverseProperty("BloodType")]
    public virtual ICollection<DonationRequest> DonationRequests { get; set; } = new List<DonationRequest>();

    [InverseProperty("BloodType")]
    public virtual ICollection<EmergencyRequest> EmergencyRequests { get; set; } = new List<EmergencyRequest>();

    [InverseProperty("BloodType")]
    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
