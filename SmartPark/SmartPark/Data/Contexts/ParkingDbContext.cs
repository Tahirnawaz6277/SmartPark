using Microsoft.EntityFrameworkCore;
using SmartPark.Models;

namespace SmartPark.Data.Contexts;

public partial class ParkingDbContext : DbContext
{
    public ParkingDbContext(DbContextOptions<ParkingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingHistory> BookingHistories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<ParkingLocation> ParkingLocations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Billing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Billing__3214EC071633A827");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking).WithMany(p => p.Billings).HasConstraintName("FK__Billing__Booking__5DCAEF64");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC071F3976DD");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Slot).WithMany(p => p.Bookings).HasConstraintName("FK__Booking__SlotId__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings).HasConstraintName("FK__Booking__UserId__5812160E");
        });

        modelBuilder.Entity<BookingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingH__3214EC0765BFFF25");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsArchived).HasDefaultValue(false);
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingHistories).HasConstraintName("FK__BookingHi__Booki__6477ECF3");

            entity.HasOne(d => d.Slot).WithMany(p => p.BookingHistories).HasConstraintName("FK__BookingHi__SlotI__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.BookingHistories).HasConstraintName("FK__BookingHi__UserI__656C112C");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC070D5AFE9B");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Location).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__Locati__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__UserId__6B24EA82");
        });

        modelBuilder.Entity<ParkingLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ParkingL__3214EC078AEAF522");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.ParkingLocations).HasConstraintName("FK__ParkingLo__UserI__4E88ABD4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC0724F0883C");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slots__3214EC0781C0686A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(d => d.Location).WithMany(p => p.Slots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Slots__LocationI__534D60F1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07FA75525E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__48CFD27E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
