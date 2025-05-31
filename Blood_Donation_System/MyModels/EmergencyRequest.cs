using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.MyModels;

public partial class EmergencyRequest
{
    [Key]
    [Column("emergency_id")]
    public int EmergencyId { get; set; }

    [Column("requester_user_id")]
    public int RequesterUserId { get; set; }

    [Column("blood_type_id")]
    public int BloodTypeId { get; set; }

    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("quantity_needed_ml")]
    public int QuantityNeededMl { get; set; }

    [Column("priority")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Priority { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("creation_date")]
    public DateTime? CreationDate { get; set; }

    [Column("fulfillment_date")]
    public DateTime? FulfillmentDate { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [ForeignKey("BloodTypeId")]
    [InverseProperty("EmergencyRequests")]
    public virtual BloodType BloodType { get; set; } = null!;

    [ForeignKey("ComponentId")]
    [InverseProperty("EmergencyRequests")]
    public virtual BloodComponent Component { get; set; } = null!;

    [InverseProperty("Emergency")]
    public virtual ICollection<EmergencyNotification> EmergencyNotifications { get; set; } = new List<EmergencyNotification>();

    [ForeignKey("RequesterUserId")]
    [InverseProperty("EmergencyRequests")]
    public virtual User RequesterUser { get; set; } = null!;
}
