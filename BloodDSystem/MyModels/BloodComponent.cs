using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("ComponentName", Name = "UQ__BloodCom__DB06D1C1ED39161A", IsUnique = true)]
public partial class BloodComponent
{
    [Key]
    [Column("ComponentID")]
    public int ComponentId { get; set; }

    [StringLength(50)]
    public string ComponentName { get; set; } = null!;

    public string? Description { get; set; }

    [InverseProperty("Component")]
    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    [InverseProperty("Component")]
    public virtual ICollection<DonationReadiness> DonationReadinesses { get; set; } = new List<DonationReadiness>();

    [InverseProperty("Component")]
    public virtual ICollection<UrgentBloodRequest> UrgentBloodRequests { get; set; } = new List<UrgentBloodRequest>();
}
