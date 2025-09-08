using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class Slot
{
    public Guid Id { get; set; }

    public string? SlotType { get; set; }

    public bool? IsAvailable { get; set; }

    public Guid LocationId { get; set; }

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ParkingLocation Location { get; set; } = null!;
}
