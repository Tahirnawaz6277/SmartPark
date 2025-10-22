using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class User
{
    public Guid Id { get; set; }

    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? City { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
    public string? ProfileImagePath { get; set; }
    public string? ImageExtension { get; set; }

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<ParkingLocation> ParkingLocations { get; set; } = new List<ParkingLocation>();

    public virtual Role Role { get; set; } = null!;
}
