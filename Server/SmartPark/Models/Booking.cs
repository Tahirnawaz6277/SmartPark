using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class Booking
{
    public Guid Id { get; set; }

    public string Duration { get; set; } = null!;

    public string BookingType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? BookingDateTime { get; set; }

    public DateTime? TimeStamp { get; set; }

    public Guid UserId { get; set; }

    public Guid SlotId { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    public virtual Slot Slot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
