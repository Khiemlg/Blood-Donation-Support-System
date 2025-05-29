using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("AudienceRole", Name = "IX_Notifications_AudienceRole")]
[Index("RecipientUserId", Name = "IX_Notifications_RecipientUserID")]
[Index("RelatedEntityType", "RelatedEntityId", Name = "IX_Notifications_RelatedEntity")]
[Index("ResponseStatus", Name = "IX_Notifications_ResponseStatus")]
[Index("SentAt", "IsRead", Name = "IX_Notifications_SentAt_IsRead")]
public partial class Notification
{
    [Key]
    [Column("NotificationID")]
    public int NotificationId { get; set; }

    [Column("RecipientUserID")]
    public int? RecipientUserId { get; set; }

    [StringLength(50)]
    public string NotificationType { get; set; } = null!;

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public bool IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SentAt { get; set; }

    [StringLength(50)]
    public string? RelatedEntityType { get; set; }

    [Column("RelatedEntityID")]
    public int? RelatedEntityId { get; set; }

    [StringLength(50)]
    public string? AudienceRole { get; set; }

    [StringLength(50)]
    public string? ResponseStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("RecipientUserId")]
    [InverseProperty("Notifications")]
    public virtual User? RecipientUser { get; set; }
}
