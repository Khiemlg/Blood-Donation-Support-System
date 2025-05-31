using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.MyModels;

public partial class DButils : DbContext
{
    public DButils()
    {
    }

    public DButils(DbContextOptions<DButils> options)
        : base(options)
    {
    }

    public virtual DbSet<BloodComponent> BloodComponents { get; set; }

    public virtual DbSet<BloodType> BloodTypes { get; set; }

    public virtual DbSet<BloodUnit> BloodUnits { get; set; }

    public virtual DbSet<DonationHistory> DonationHistories { get; set; }

    public virtual DbSet<DonationRequest> DonationRequests { get; set; }

    public virtual DbSet<EmergencyNotification> EmergencyNotifications { get; set; }

    public virtual DbSet<EmergencyRequest> EmergencyRequests { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__BloodCom__AEB1DA59A44C9312");
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId).HasName("PK__BloodTyp__56FFB8C8AEEE087F");
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__BloodUni__D3AF5BD718FC5A6B");

            entity.Property(e => e.Status).HasDefaultValue("Available");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__blood__5AEE82B9");

            entity.HasOne(d => d.Component).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__compo__5BE2A6F2");

            entity.HasOne(d => d.Donation).WithMany(p => p.BloodUnits).HasConstraintName("FK__BloodUnit__donat__59FA5E80");
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PK__Donation__296B91DCFDF8D202");

            entity.Property(e => e.Status).HasDefaultValue("Completed");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__blood__5441852A");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__compo__5535A963");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.DonationHistoryDonorUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__donor__534D60F1");

            entity.HasOne(d => d.StaffUser).WithMany(p => p.DonationHistoryStaffUsers).HasConstraintName("FK__DonationH__staff__5629CD9C");
        });

        modelBuilder.Entity<DonationRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Donation__18D3B90F7590CC25");

            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__blood__4E88ABD4");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__compo__4F7CD00D");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__donor__4D94879B");
        });

        modelBuilder.Entity<EmergencyNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Emergenc__E059842FB6D2AB1F");

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ResponseStatus).HasDefaultValue("No Response");
            entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Emergency).WithMany(p => p.EmergencyNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__emerg__68487DD7");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.EmergencyNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__recip__693CA210");
        });

        modelBuilder.Entity<EmergencyRequest>(entity =>
        {
            entity.HasKey(e => e.EmergencyId).HasName("PK__Emergenc__F0E90B91A7A18C89");

            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Priority).HasDefaultValue("Medium");

            entity.HasOne(d => d.BloodType).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__blood__619B8048");

            entity.HasOne(d => d.Component).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__compo__628FA481");

            entity.HasOne(d => d.RequesterUser).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__reque__60A75C0F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F4E0E4754");

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__recip__6E01572D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CCE2BB0BCA");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F104E87E2");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__440B1D61");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__AEBB701F006AAFFD");

            entity.HasOne(d => d.BloodType).WithMany(p => p.UserProfiles).HasConstraintName("FK__UserProfi__blood__48CFD27E");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProfi__user___47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
