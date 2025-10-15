using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class Billing
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public DateTime? TimeStamp { get; set; }

    public Guid BookingId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
