using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.BusinessLogic.MyModels;

public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public String NotificationId { get; set; }

    [Column("recipient_user_id")]
    public String RecipientUserId { get; set; }

    [Column("message")]
    public string Message { get; set; } = null!;

    [Column("type")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Type { get; set; }

    [Column("sent_date")]
    public DateTime? SentDate { get; set; }

    [Column("is_read")]
    public bool? IsRead { get; set; }

    [ForeignKey("RecipientUserId")]
    [InverseProperty("Notifications")]
    public virtual User RecipientUser { get; set; } = null!;
}
