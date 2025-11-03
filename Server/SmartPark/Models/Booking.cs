using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class Booking
{
    public Guid Id { get; set; }

    public string Status { get; set; } = null!;

    public Guid UserId { get; set; }

    //public Guid SlotId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Guid? CancelledBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    // One Booking -> Many Slots
    public ICollection<Slot> Slots { get; set; }   

    public virtual User User { get; set; } = null!;
}
