using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

[Table("BookingHistory")]
public partial class BookingHistory
{
    [Key]
    public Guid Id { get; set; }

    public int? Duration { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndedAt { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? StatusSnapshot { get; set; }

    public bool? IsArchived { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeStamp { get; set; }

    public Guid? SlotId { get; set; }

    public Guid? BookingId { get; set; }

    public Guid? UserId { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingHistories")]
    public virtual Booking? Booking { get; set; }

    [ForeignKey("SlotId")]
    [InverseProperty("BookingHistories")]
    public virtual Slot? Slot { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("BookingHistories")]
    public virtual User? User { get; set; }
}
