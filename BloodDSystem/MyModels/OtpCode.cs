using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("Email", Name = "IX_OtpCodes_Email")]
[Index("ExpiresAt", "IsUsed", Name = "IX_OtpCodes_ExpiresAt_IsUsed")]
[Index("UserId", Name = "IX_OtpCodes_UserID")]
public partial class OtpCode
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(10)]
    public string Otp { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("OtpCodes")]
    public virtual User? User { get; set; }
}
