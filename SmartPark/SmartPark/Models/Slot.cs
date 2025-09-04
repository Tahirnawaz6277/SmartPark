using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

public partial class Slot
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SlotType { get; set; }

    public bool? IsAvailable { get; set; }

    public Guid LocationId { get; set; }

    [InverseProperty("Slot")]
    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    [InverseProperty("Slot")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("LocationId")]
    [InverseProperty("Slots")]
    public virtual ParkingLocation Location { get; set; } = null!;
}
