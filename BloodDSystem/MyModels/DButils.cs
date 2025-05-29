using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BloodDSystem.MyModels;

public partial class DButils : DbContext
{
    public DButils()
    {
    }

    public DButils(DbContextOptions<DButils> options)
        : base(options)
    {
    }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<BloodComponent> BloodComponents { get; set; }

    public virtual DbSet<BloodType> BloodTypes { get; set; }

    public virtual DbSet<BloodUnit> BloodUnits { get; set; }

    public virtual DbSet<DonationApplication> DonationApplications { get; set; }

    public virtual DbSet<DonationProcess> DonationProcesses { get; set; }

    public virtual DbSet<DonationReadiness> DonationReadinesses { get; set; }

    public virtual DbSet<MemberProfile> MemberProfiles { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OtpCode> OtpCodes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UrgentBloodRequest> UrgentBloodRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=LAPTOP-CMSU7LK1;database=BloodDonationSystem; uid=sa;pwd=12345; encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__BlogPost__AA126038F9DFF9F5");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.BlogPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlogPosts_CreatedByUser");
        });

        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__BloodCom__D79CF02E61894DB6");
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId).HasName("PK__BloodTyp__B489BA43DCC18056");
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__BloodUni__44F5EC95AEE8C0FB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodUnits_BloodType");

            entity.HasOne(d => d.CollectedByUser).WithMany(p => p.BloodUnitCollectedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodUnits_CollectedByUser");

            entity.HasOne(d => d.Component).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodUnits_BloodComponent");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.BloodUnitDonorUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodUnits_DonorUser");
        });

        modelBuilder.Entity<DonationApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Donation__C93A4F79D142E726");

            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationApplications).HasConstraintName("FK_DonationApplications_BloodType");

            entity.HasOne(d => d.ReviewedByUser).WithMany(p => p.DonationApplicationReviewedByUsers).HasConstraintName("FK_DonationApplications_ReviewedByUser");

            entity.HasOne(d => d.User).WithMany(p => p.DonationApplicationUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationApplications_User");
        });

        modelBuilder.Entity<DonationProcess>(entity =>
        {
            entity.HasKey(e => e.ProcessId).HasName("PK__Donation__1B39A97653F875F2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StartTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ConnectedDonorUser).WithMany(p => p.DonationProcessConnectedDonorUsers).HasConstraintName("FK_DonationProcesses_ConnectedDonorUser");

            entity.HasOne(d => d.ManagedByUser).WithMany(p => p.DonationProcessManagedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationProcesses_ManagedByUser");

            entity.HasOne(d => d.Request).WithMany(p => p.DonationProcesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationProcesses_Request");

            entity.HasOne(d => d.UsedBloodUnit).WithMany(p => p.DonationProcesses).HasConstraintName("FK_DonationProcesses_UsedBloodUnit");
        });

        modelBuilder.Entity<DonationReadiness>(entity =>
        {
            entity.HasKey(e => e.DonationReadinessId).HasName("PK__Donation__63A7C6A0773626B0");

            entity.Property(e => e.ConfirmedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationReadinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationReadiness_BloodType");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationReadinesses).HasConstraintName("FK_DonationReadiness_BloodComponent");

            entity.HasOne(d => d.ConfirmedByUser).WithMany(p => p.DonationReadinessConfirmedByUsers).HasConstraintName("FK_DonationReadiness_ConfirmedByUser");

            entity.HasOne(d => d.User).WithMany(p => p.DonationReadinessUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationReadiness_User");
        });

        modelBuilder.Entity<MemberProfile>(entity =>
        {
            entity.HasKey(e => e.MemberProfileId).HasName("PK__MemberPr__048520BF42C58B67");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.BloodType).WithMany(p => p.MemberProfiles).HasConstraintName("FK_MemberProfiles_BloodType");

            entity.HasOne(d => d.User).WithOne(p => p.MemberProfile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberProfiles_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E3256545D93");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.Notifications).HasConstraintName("FK_Notifications_RecipientUser");
        });

        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.OtpCodes).HasConstraintName("FK_OtpCodes_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A81AD47C1");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<UrgentBloodRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__UrgentBl__33A8519AEC2832B5");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RequestedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.UrgentBloodRequestApprovedByUsers).HasConstraintName("FK_UrgentBloodRequests_ApprovedByUser");

            entity.HasOne(d => d.BloodType).WithMany(p => p.UrgentBloodRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UrgentBloodRequests_BloodType");

            entity.HasOne(d => d.Component).WithMany(p => p.UrgentBloodRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UrgentBloodRequests_BloodComponent");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.UrgentBloodRequestCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UrgentBloodRequests_CreatedByUser");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC68BAAD40");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Role");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__290C8884ED8F3C2D");

            entity.Property(e => e.City).HasDefaultValue("Ho Chi Minh City");
            entity.Property(e => e.Country).HasDefaultValue("Vietnam");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
