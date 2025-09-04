using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartPark.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Description { get; set; }

    public int? Rating { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeStamp { get; set; }

    public Guid? UserId { get; set; }

    public Guid? LocationId { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("Feedbacks")]
    public virtual ParkingLocation? Location { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Feedbacks")]
    public virtual User? User { get; set; }
}
