using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

[Table("ParkingLocation")]
public partial class ParkingLocation
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Name { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Address { get; set; }

    public int? TotalSlots { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? City { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Image { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeStamp { get; set; }

    public Guid? UserId { get; set; }

    [InverseProperty("Location")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Location")]
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    [ForeignKey("UserId")]
    [InverseProperty("ParkingLocations")]
    public virtual User? User { get; set; }
}
