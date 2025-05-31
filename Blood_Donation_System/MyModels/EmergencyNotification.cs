using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.MyModels;

public partial class EmergencyNotification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    [Column("emergency_id")]
    public int EmergencyId { get; set; }

    [Column("recipient_user_id")]
    public int RecipientUserId { get; set; }

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
