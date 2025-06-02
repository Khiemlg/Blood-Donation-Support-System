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
            entity.HasKey(e => e.ComponentId);
            entity.Property(e => e.ComponentName)
                    .IsRequired()
                    .HasMaxLength(50);
            entity.HasIndex(e => e.ComponentName).IsUnique();
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId);
            entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(10);
            entity.HasIndex(e => e.TypeName).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

            entity.Property(e => e.Email)
                    .HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            entity.Property(e => e.RegistrationDate)
                    .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId);
            entity.Property(e => e.ProfileId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

            entity.Property(e => e.Cccd)
                    .HasMaxLength(20);
            entity.HasIndex(e => e.Cccd).IsUnique();

            entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);
            entity.HasIndex(e => e.PhoneNumber).IsUnique();

            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(11, 8)");

            entity.HasOne(d => d.BloodType)
                .WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserProfile!)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();
        });

        modelBuilder.Entity<DonationRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId);
            entity.Property(e => e.RequestId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");
            entity.Property(e => e.RequestDate)
                    .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.BloodType)
                .WithMany(p => p.DonationRequests)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.Component)
                .WithMany(p => p.DonationRequests)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.DonorUser)
                .WithMany(p => p.DonationRequests)
                .HasForeignKey(d => d.DonorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(e => e.DonationId);
            entity.Property(e => e.DonationId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.DonationDate).IsRequired();
            entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Completed");

            entity.HasOne(d => d.BloodType)
                .WithMany(p => p.DonationHistories)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.Component)
                .WithMany(p => p.DonationHistories)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.DonorUser)
                .WithMany(p => p.DonationHistoryDonorUsers)
                .HasForeignKey(d => d.DonorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.StaffUser)
                .WithMany(p => p.DonationHistoryStaffUsers)
                .HasForeignKey(d => d.StaffUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId);
            entity.Property(e => e.UnitId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.VolumeMl).IsRequired();
            entity.Property(e => e.CollectionDate).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Available");

            entity.HasOne(d => d.BloodType)
                .WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.Component)
                .WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.Donation)
                .WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.DonationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmergencyRequest>(entity =>
        {
            entity.HasKey(e => e.EmergencyId);
            entity.Property(e => e.EmergencyId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.QuantityNeededMl).IsRequired();
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.Priority)
                    .HasMaxLength(10)
                    .HasDefaultValue("Medium");
            entity.Property(e => e.CreationDate)
                    .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.BloodType)
                .WithMany(p => p.EmergencyRequests)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.Component)
                .WithMany(p => p.EmergencyRequests)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.RequesterUser)
                .WithMany(p => p.EmergencyRequests)
                .HasForeignKey(d => d.RequesterUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();
        });

        modelBuilder.Entity<EmergencyNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.DeliveryMethod)
                    .IsRequired()
                    .HasMaxLength(20);
            entity.Property(e => e.IsRead)
                    .HasDefaultValue(false);
            entity.Property(e => e.ResponseStatus)
                    .HasMaxLength(20);
            entity.Property(e => e.SentDate)
                    .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Emergency)
                .WithMany(p => p.EmergencyNotifications)
                .HasForeignKey(d => d.EmergencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            entity.HasOne(d => d.RecipientUser)
                .WithMany(p => p.EmergencyNotifications)
                .HasForeignKey(d => d.RecipientUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationId)
                  .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.IsRead)
                    .HasDefaultValue(false);
            entity.Property(e => e.SentDate)
                    .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.RecipientUser)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RecipientUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
