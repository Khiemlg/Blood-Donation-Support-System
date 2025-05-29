using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

[Index("CreatedByUserId", Name = "IX_BlogPosts_CreatedByUserID")]
[Index("PostType", Name = "IX_BlogPosts_PostType")]
public partial class BlogPost
{
    [Key]
    [Column("PostID")]
    public int PostId { get; set; }

    [StringLength(500)]
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    [StringLength(50)]
    public string PostType { get; set; } = null!;

    [Column("CreatedByUserID")]
    public int CreatedByUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("BlogPosts")]
    public virtual User CreatedByUser { get; set; } = null!;
}
