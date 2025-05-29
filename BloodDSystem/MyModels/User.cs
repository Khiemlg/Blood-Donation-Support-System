using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("Email", Name = "IX_Users_Email")]
[Index("RoleId", Name = "IX_Users_RoleID")]
[Index("Email", Name = "UQ__Users__A9D1053481FF78E1", IsUnique = true)]
public partial class User
{
    internal readonly bool IsEmailVerified;

    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(255)]
    public string FullName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    [InverseProperty("CollectedByUser")]
    public virtual ICollection<BloodUnit> BloodUnitCollectedByUsers { get; set; } = new List<BloodUnit>();

    [InverseProperty("DonorUser")]
    public virtual ICollection<BloodUnit> BloodUnitDonorUsers { get; set; } = new List<BloodUnit>();

    [InverseProperty("ReviewedByUser")]
    public virtual ICollection<DonationApplication> DonationApplicationReviewedByUsers { get; set; } = new List<DonationApplication>();

    [InverseProperty("User")]
    public virtual ICollection<DonationApplication> DonationApplicationUsers { get; set; } = new List<DonationApplication>();

    [InverseProperty("ConnectedDonorUser")]
    public virtual ICollection<DonationProcess> DonationProcessConnectedDonorUsers { get; set; } = new List<DonationProcess>();

    [InverseProperty("ManagedByUser")]
    public virtual ICollection<DonationProcess> DonationProcessManagedByUsers { get; set; } = new List<DonationProcess>();

    [InverseProperty("ConfirmedByUser")]
    public virtual ICollection<DonationReadiness> DonationReadinessConfirmedByUsers { get; set; } = new List<DonationReadiness>();

    [InverseProperty("User")]
    public virtual ICollection<DonationReadiness> DonationReadinessUsers { get; set; } = new List<DonationReadiness>();

    [InverseProperty("User")]
    public virtual MemberProfile? MemberProfile { get; set; }

    [InverseProperty("RecipientUser")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("ApprovedByUser")]
    public virtual ICollection<UrgentBloodRequest> UrgentBloodRequestApprovedByUsers { get; set; } = new List<UrgentBloodRequest>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<UrgentBloodRequest> UrgentBloodRequestCreatedByUsers { get; set; } = new List<UrgentBloodRequest>();

    [InverseProperty("User")]
    public virtual UserProfile? UserProfile { get; set; }
}
