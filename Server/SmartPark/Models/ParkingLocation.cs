using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class ParkingLocation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int TotalSlots { get; set; }

    public string? City { get; set; }

    public string? Image { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual User User { get; set; } = null!;
}
