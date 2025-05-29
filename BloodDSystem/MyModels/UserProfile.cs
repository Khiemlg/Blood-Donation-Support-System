using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("Latitude", "Longitude", Name = "IX_UserProfiles_Latitude_Longitude")]
[Index("UserId", Name = "UQ__UserProf__1788CCADED20AAC3", IsUnique = true)]
public partial class UserProfile
{
    [Key]
    [Column("ProfileID")]
    public int ProfileId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(500)]
    public string? DetailedAddress { get; set; }

    [StringLength(100)]
    public string? Ward { get; set; }

    [StringLength(100)]
    public string? District { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? Longitude { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserProfile")]
    public virtual User User { get; set; } = null!;
}
