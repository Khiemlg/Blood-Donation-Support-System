using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Model.Enties;

[Index("ComponentName", Name = "UQ__BloodCom__2E7CCD4B3D5750C3", IsUnique = true)]
public partial class BloodComponent
{
    [Key]
    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("component_name")]
    [StringLength(50)]
    [Unicode(false)]
    public string ComponentName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [InverseProperty("Component")]
    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    [InverseProperty("Component")]
    public virtual ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();

    [InverseProperty("Component")]
    public virtual ICollection<DonationRequest> DonationRequests { get; set; } = new List<DonationRequest>();

    [InverseProperty("Component")]
    public virtual ICollection<EmergencyRequest> EmergencyRequests { get; set; } = new List<EmergencyRequest>();
}
