using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class Slot
{
    public Guid Id { get; set; }

    public string SlotNumber { get; set; } = null!;

    public bool IsAvailable { get; set; } = false;

    public Guid LocationId { get; set; }
    public Guid? BookingId { get; set; }

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();
    public virtual Booking Booking { get; set; }

    public virtual ParkingLocation Location { get; set; } = null!;
}
