using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Model.Enties;

public partial class EmergencyNotification
{
    [Key]
    [Column("notification_id")]
    [StringLength(36)]
    public string NotificationId { get; set; } = null!;

    [Column("emergency_id")]
    [StringLength(36)]
    public string EmergencyId { get; set; } = null!;

    [Column("recipient_user_id")]
    [StringLength(36)]
    public string RecipientUserId { get; set; } = null!;

    [Column("sent_date")]
    public DateTime? SentDate { get; set; }

    [Column("delivery_method")]
    [StringLength(20)]
    [Unicode(false)]
    public string DeliveryMethod { get; set; } = null!;

    [Column("is_read")]
    public bool? IsRead { get; set; }

    [Column("response_status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ResponseStatus { get; set; }

    [ForeignKey("EmergencyId")]
    [InverseProperty("EmergencyNotifications")]
    public virtual EmergencyRequest Emergency { get; set; } = null!;

    [ForeignKey("RecipientUserId")]
    [InverseProperty("EmergencyNotifications")]
    public virtual User RecipientUser { get; set; } = null!;
}
