using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

[Table("Booking")]
public partial class Booking
{
    [Key]
    public Guid Id { get; set; }

    public int? Duration { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? BookingDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ParkingStartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ParkingEndTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeStamp { get; set; }

    public Guid? UserId { get; set; }

    public Guid? SlotId { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    [InverseProperty("Booking")]
    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    [ForeignKey("SlotId")]
    [InverseProperty("Bookings")]
    public virtual Slot? Slot { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User? User { get; set; }
}
