using System;
using System.Collections.Generic;
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

            entity.ToTable("Billing");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Billings)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Billing__Booking__5DCAEF64");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC071F3976DD");

            entity.ToTable("Booking");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Slot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__SlotId__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__UserId__5812160E");
        });

        modelBuilder.Entity<BookingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingH__3214EC0765BFFF25");

            entity.ToTable("BookingHistory");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.StatusSnapshot)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingHistories)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingHi__Booki__6477ECF3");

            entity.HasOne(d => d.Slot).WithMany(p => p.BookingHistories)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingHi__SlotI__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.BookingHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingHi__UserI__656C112C");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC070D5AFE9B");

            entity.ToTable("Feedback");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TimeStamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Location).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__Feedback__Locati__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedback__UserId__6B24EA82");
        });

        modelBuilder.Entity<ParkingLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ParkingL__3214EC078AEAF522");

            entity.ToTable("ParkingLocation");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.ParkingLocations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingLo__UserI__4E88ABD4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC0724F0883C");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slots__3214EC0781C0686A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.SlotNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Location).WithMany(p => p.Slots)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__Slots__LocationI__534D60F1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07FA75525E");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534CBBEE4F1").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__48CFD27E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
