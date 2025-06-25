
using BloodDonation_System.Model.Enties;
using Microsoft.EntityFrameworkCore;


namespace BloodDonation_System.Data;

public partial class DButils : DbContext
{ //kmn
    public DButils()
    {
    }

    public DButils(DbContextOptions<DButils> options)
        : base(options)
    {
    }
    public DbSet<ReminderLog> ReminderLogs { get; set; }

    public virtual DbSet<OtpCode> OtpCodes { get; set; }

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
    public DbSet<DonationHistory> DonationHistory { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=FPTSTUDENTS;database=bloodDSystem; uid=sa;pwd=12345; encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__BloodCom__AEB1DA593FA76C7E");
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId).HasName("PK__BloodTyp__56FFB8C844AAA67A");
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__BloodUni__D3AF5BD712E32CB4");

            entity.Property(e => e.UnitId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Status).HasDefaultValue("Available");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__blood__4E88ABD4");

            entity.HasOne(d => d.Component).WithMany(p => p.BloodUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloodUnit__compo__4F7CD00D");

            entity.HasOne(d => d.Donation).WithMany(p => p.BloodUnits).HasConstraintName("FK__BloodUnit__donat__4D94879B");
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PK__Donation__296B91DC9926847F");

            entity.Property(e => e.DonationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Status).HasDefaultValue("Completed");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__blood__46E78A0C");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__compo__47DBAE45");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.DonationHistoryDonorUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationH__donor__45F365D3");

            entity.HasOne(d => d.Emergency).WithMany(p => p.DonationHistories).HasConstraintName("FK_DonationHistory_EmergencyRequests");

            entity.HasOne(d => d.StaffUser).WithMany(p => p.DonationHistoryStaffUsers).HasConstraintName("FK__DonationH__staff__48CFD27E");
        });

        modelBuilder.Entity<DonationRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Donation__18D3B90F6EEDB784");

            entity.Property(e => e.RequestId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.BloodType).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__blood__403A8C7D");

            entity.HasOne(d => d.Component).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__compo__412EB0B6");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.DonationRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonationR__donor__3F466844");
        });

        modelBuilder.Entity<EmergencyNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Emergenc__E059842F9E58D36A");

            entity.Property(e => e.NotificationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ResponseStatus).HasDefaultValue("No Response");
            entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Emergency).WithMany(p => p.EmergencyNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__emerg__5DCAEF64");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.EmergencyNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__recip__5EBF139D");
        });

        modelBuilder.Entity<EmergencyRequest>(entity =>
        {
            entity.HasKey(e => e.EmergencyId).HasName("PK__Emergenc__F0E90B912572BA0B");

            entity.Property(e => e.EmergencyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Priority).HasDefaultValue("Medium");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.BloodType).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__blood__5629CD9C");

            entity.HasOne(d => d.Component).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__compo__571DF1D5");

            entity.HasOne(d => d.RequesterUser).WithMany(p => p.EmergencyRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Emergency__reque__5535A963");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F3723CF21");

            entity.Property(e => e.NotificationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__recip__6477ECF3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC49C11010");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F03DE91F1");

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__31EC6D26");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__AEBB701F14C8FF44");

            entity.Property(e => e.ProfileId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.BloodType).WithMany(p => p.UserProfiles).HasConstraintName("FK__UserProfi__blood__398D8EEE");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProfi__user___38996AB5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
