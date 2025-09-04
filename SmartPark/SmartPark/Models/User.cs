using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

[Table("User")]
[Index("Email", Name = "UQ__User__A9D10534CBBEE4F1", IsUnique = true)]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    public Guid RoleId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? Address { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? City { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    [InverseProperty("User")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("User")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("User")]
    public virtual ICollection<ParkingLocation> ParkingLocations { get; set; } = new List<ParkingLocation>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
