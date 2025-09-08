using System;
using System.Collections.Generic;

namespace SmartPark.Models;

public partial class Feedback
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public int? Rating { get; set; }

    public DateTime? TimeStamp { get; set; }

    public Guid? UserId { get; set; }

    public Guid? LocationId { get; set; }

    public virtual ParkingLocation? Location { get; set; }

    public virtual User? User { get; set; }
}
