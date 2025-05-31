using System;
using System.Collections.Generic;
using Blood_Donation_System.BusinessLogic.MyModels;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.DataAccess;

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
            entity.HasKey(e => e.ComponentId); // Removed .HasName()
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId); // Removed .HasName()
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.UnitId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.DonationId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Status).HasDefaultValue("Available");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__blood__5AEE82B9");

            entity.HasOne(d => d.Component).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__compo__5BE2A6F2");

            entity.HasOne(d => d.Donation).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.DonationId) // Changed from int? to string?
                .HasConstraintName("FK__BloodUnit__donat__59FA5E80");
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(e => e.DonationId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.DonationId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.DonorUserId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.StaffUserId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.EmergencyId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.Status).HasDefaultValue("Completed");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__blood__5441852A");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__compo__5535A963");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.DonationHistoryDonorUsers)
                .HasForeignKey(d => d.DonorUserId) // Changed from int to string
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__donor__534D60F1");

            entity.HasOne(d => d.StaffUser).WithMany(p => p.DonationHistoryStaffUsers)
                .HasForeignKey(d => d.StaffUserId) // Changed from int? to string?
                .HasConstraintName("FK__DonationH__staff__5629CD9C");

            entity.HasOne(d => d.Emergency).WithMany(p => p.DonationHistories)
                .HasForeignKey(d => d.EmergencyId)
                .HasConstraintName("FK__DonationH__emergency__...some_name..."); // You'll need the actual name
        });

        modelBuilder.Entity<DonationRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.RequestId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.DonorUserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__blood__4E88ABD4");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__compo__4F7CD00D");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.DonationRequests)
                .HasForeignKey(d => d.DonorUserId) // Changed from int to string
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__donor__4D94879B");
        });

        modelBuilder.Entity<EmergencyNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.NotificationId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.EmergencyId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.RecipientUserId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ResponseStatus).HasDefaultValue("No Response");
            entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Emergency).WithMany(p => p.EmergencyNotifications)
                .HasForeignKey(d => d.EmergencyId) // Changed from int to string
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__emerg__68487DD7");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.EmergencyNotifications)
                .HasForeignKey(d => d.RecipientUserId) // Changed from int to string
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__recip__693CA210");
        });

        modelBuilder.Entity<EmergencyRequest>(entity =>
        {
            entity.HasKey(e => e.EmergencyId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.EmergencyId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RequesterUserId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Priority).HasDefaultValue("Medium");

            entity.HasOne(d => d.BloodType).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__blood__619B8048");

            entity.HasOne(d => d.Component).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__compo__628FA481");

            entity.HasOne(d => d.RequesterUser).WithMany(p => p.EmergencyRequests)
                .HasForeignKey(d => d.RequesterUserId) // Changed from int to string
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__reque__60A75C0F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.NotificationId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RecipientUserId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RecipientUserId) // Changed from int to string
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__recip__6E01572D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId); // Removed .HasName()
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.RoleId).IsRequired(); // RoleId is required, so make it non-nullable

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__440B1D61");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId); // Changed from int to string, Removed .HasName()

            entity.Property(e => e.ProfileId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.UserId)
               .HasMaxLength(50)
               .IsUnicode(false);

            entity.HasOne(d => d.BloodType).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.BloodTypeId)
                .HasConstraintName("FK__UserProfi__blood__48CFD27E");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId) // Explicitly specify FK property
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProfi__user___47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}