using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Model.Enties;

[Index("Email", Name = "UQ__Users__AB6E6164C2A997BF", IsUnique = true)]
[Index("Username", Name = "UQ__Users__F3DBC5720B82F2C7", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    [StringLength(36)]
    public string UserId { get; set; } = null!;

    [Column("username")]
    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("registration_date")]
    public DateTime? RegistrationDate { get; set; }

    [Column("last_login_date")]
    public DateTime? LastLoginDate { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [NotMapped]
    public string RoleName
    {
        get { return Role?.RoleName; }
        set { }
    }

    [InverseProperty("DonorUser")]
    public virtual ICollection<DonationHistory> DonationHistoryDonorUsers { get; set; } = new List<DonationHistory>();

    [InverseProperty("StaffUser")]
    public virtual ICollection<DonationHistory> DonationHistoryStaffUsers { get; set; } = new List<DonationHistory>();

    [InverseProperty("DonorUser")]
    public virtual ICollection<DonationRequest> DonationRequests { get; set; } = new List<DonationRequest>();

    [InverseProperty("RecipientUser")]
    public virtual ICollection<EmergencyNotification> EmergencyNotifications { get; set; } = new List<EmergencyNotification>();

    [InverseProperty("RequesterUser")]
    public virtual ICollection<EmergencyRequest> EmergencyRequests { get; set; } = new List<EmergencyRequest>();

    [InverseProperty("RecipientUser")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual UserProfile? UserProfile { get; set; }
}
