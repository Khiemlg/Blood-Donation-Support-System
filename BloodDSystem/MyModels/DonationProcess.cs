using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("ManagedByUserId", Name = "IX_DonationProcesses_ManagedByUserID")]
[Index("RequestId", Name = "IX_DonationProcesses_RequestID")]
public partial class DonationProcess
{
    [Key]
    [Column("ProcessID")]
    public int ProcessId { get; set; }

    [Column("RequestID")]
    public int RequestId { get; set; }

    [Column("ConnectedDonorUserID")]
    public int? ConnectedDonorUserId { get; set; }

    [Column("UsedBloodUnitID")]
    public int? UsedBloodUnitId { get; set; }

    [Column("ManagedByUserID")]
    public int ManagedByUserId { get; set; }

    [StringLength(50)]
    public string ProcessStatus { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ConnectedDonorUserId")]
    [InverseProperty("DonationProcessConnectedDonorUsers")]
    public virtual User? ConnectedDonorUser { get; set; }

    [ForeignKey("ManagedByUserId")]
    [InverseProperty("DonationProcessManagedByUsers")]
    public virtual User ManagedByUser { get; set; } = null!;

    [ForeignKey("RequestId")]
    [InverseProperty("DonationProcesses")]
    public virtual UrgentBloodRequest Request { get; set; } = null!;

    [ForeignKey("UsedBloodUnitId")]
    [InverseProperty("DonationProcesses")]
    public virtual BloodUnit? UsedBloodUnit { get; set; }
}
