using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("BloodTypeName", Name = "UQ__BloodTyp__3323606B8213E8D3", IsUnique = true)]
public partial class BloodType
{
    [Key]
    [Column("BloodTypeID")]
    public int BloodTypeId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string BloodTypeName { get; set; } = null!;

    public string? Description { get; set; }

    [InverseProperty("BloodType")]
    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    [InverseProperty("BloodType")]
    public virtual ICollection<DonationApplication> DonationApplications { get; set; } = new List<DonationApplication>();

    [InverseProperty("BloodType")]
    public virtual ICollection<DonationReadiness> DonationReadinesses { get; set; } = new List<DonationReadiness>();

    [InverseProperty("BloodType")]
    public virtual ICollection<MemberProfile> MemberProfiles { get; set; } = new List<MemberProfile>();

    [InverseProperty("BloodType")]
    public virtual ICollection<UrgentBloodRequest> UrgentBloodRequests { get; set; } = new List<UrgentBloodRequest>();
}
