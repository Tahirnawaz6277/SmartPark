using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

[Table("Billing")]
public partial class Billing
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Amount { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PaymentStatus { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PaymentMethod { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeStamp { get; set; }

    public Guid? BookingId { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Billings")]
    public virtual Booking? Booking { get; set; }
}
