using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("RoleName", Name = "UQ__Roles__8A2B6160A3284DFC", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
