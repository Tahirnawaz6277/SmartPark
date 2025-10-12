using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class BookingHistory
{
    public Guid Id { get; set; }

    public int Duration { get; set; }

    public string BookingType { get; set; } = null!;

    public string? StatusSnapshot { get; set; }

    public DateTime? TimeStamp { get; set; }

    public Guid SlotId { get; set; }

    public Guid BookingId { get; set; }

    public Guid UserId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
